using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class FourBitTile : Tile {
    public Sprite[] sprites = new Sprite[16];
    public Sprite previewSprite;

    public override void RefreshTile( Vector3Int position, ITilemap tilemap ) {
        for( int yd = -1; yd <= 1; yd++ ) {
            for( int xd = -1; xd <= 1; xd++ ) {
                Vector3Int pos = new Vector3Int(position.x + xd, position.y + yd, position.z);
                if( HasWallTile( tilemap, pos ) ) {
                    tilemap.RefreshTile( pos );
                }
            }
        }
    }

    public override void GetTileData( Vector3Int position, ITilemap tilemap, ref TileData tileData ) {
        int mask = HasWallTile(tilemap, position + new Vector3Int(0, 1, 0)) ? 1 : 0; //UP
        mask += HasWallTile( tilemap, position + new Vector3Int( -1, 0, 0 ) ) ? 2 : 0; //LEFT
        mask += HasWallTile( tilemap, position + new Vector3Int( 1, 0, 0 ) ) ? 4 : 0; //RIGHT
        mask += HasWallTile( tilemap, position + new Vector3Int( 0, -1, 0 ) ) ? 8 : 0; //DOWN

        int index = GetIndex((byte) mask);

        if( index >= 0 && index < sprites.Length ) {
            tileData.sprite = sprites[index];
            tileData.color = Color.white;
            tileData.transform.SetTRS( Vector3.zero, Quaternion.identity, Vector3.one );
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.None;
        } else {
            Debug.LogWarning( "Not enough sprites in FourBitTile instance" );
        }
    }

    private int GetIndex( byte mask ) {
        switch( mask ) {
            case 0: return 0;
            case 1: return 1;
            case 2: return 2;
            case 3: return 3;
            case 4: return 4;
            case 5: return 5;
            case 6: return 6;
            case 7: return 7;
            case 8: return 8;
            case 9: return 9;
            case 10: return 10;
            case 11: return 11;
            case 12: return 12;
            case 13: return 13;
            case 14: return 14;
            case 15: return 15;
        }
        return -1;
    }

    private bool HasWallTile( ITilemap tilemap, Vector3Int position ) {
        return tilemap.GetTile( position ) == this;
    }

#if UNITY_EDITOR

    [MenuItem( "Tile Assets/Create/4-Bit Tile" )]
    public static void CreateWallTile() {
        string path = EditorUtility.SaveFilePanelInProject("Save 4-Bit Tile", "New 4-Bit Tile", "asset", "Save 4-Bit Tile", "Assets");
        if( path == "" ) {
            return;
        }
        AssetDatabase.CreateAsset( ScriptableObject.CreateInstance<FourBitTile>(), path );
    }

#endif
}