using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class EightBitTile : Tile {
    public Sprite[] sprites = new Sprite[48];
    public Sprite previewSprite;

    Vector3Int TOP_LEFT = new Vector3Int(-1, 1, 0);
    Vector3Int TOP_RIGHT = new Vector3Int(1, 1, 0);
    Vector3Int BOTTOM_LEFT = new Vector3Int(-1, -1, 0);
    Vector3Int BOTTOM_RIGHT = new Vector3Int(1, -1, 0);
    Vector3Int TOP = new Vector3Int(0, 1, 0);
    Vector3Int BOTTOM = new Vector3Int(0, -1, 0);
    Vector3Int LEFT = new Vector3Int(-1, 0, 0);
    Vector3Int RIGHT = new Vector3Int(1, 0, 0);

    //126 -> 32 (should be 30)
    //122 -> 30 (should be 32)
    //91 -> 23 (should be 39)
    //219 -> 39 (should be 23?)
    //
    Dictionary<int, int> indexDictionary = new Dictionary<int, int>()
            {{2,1 }, {8, 2 }, {10,3 }, {11,4 }, {16, 5 }, {18, 6 }, {22, 7 }, {24, 8 },
            {26,9 }, {27, 10 }, {30, 11 }, {31, 12 }, {64,13 }, {66, 14 }, {72, 15 },
            { 74, 16}, { 75, 17 }, {80, 18 }, {82, 19 }, {86, 20 }, {88, 21 }, {90, 22 },
            {91, 39 }, {94, 24 }, {95, 25 },{ 104 , 26 }, {106, 27 }, {107, 28 }, {120, 29 },
            {122, 32 }, {123, 31 }, {126 , 30 }, {127, 33 }, {208, 34 }, {210,35 }, {214, 36 },
            {216, 37 }, {218, 38 }, {219 , 23 }, {222, 40 }, {223, 41 }, {248, 42 }, {250, 43 },
            {251, 44 }, {254, 45 }, {255, 46 }, {0, 47 } };

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
        int mask = HasWallTile(tilemap, position + new Vector3Int(-1, 1, 0), TOP_LEFT) ? 1 : 0; //UP-LEFT

        mask += HasWallTile( tilemap, position + new Vector3Int( 0, 1, 0 ) ) ? 2 : 0; //UP

        mask += HasWallTile( tilemap, position + new Vector3Int( 1, 1, 0 ), TOP_RIGHT ) ? 4 : 0; //UP-RIGHT

        mask += HasWallTile( tilemap, position + new Vector3Int( -1, 0, 0 ) ) ? 8 : 0; //LEFT

        mask += HasWallTile( tilemap, position + new Vector3Int( 1, 0, 0 ) ) ? 16 : 0;  //RIGHT

        mask += HasWallTile( tilemap, position + new Vector3Int( -1, -1, 0 ), BOTTOM_LEFT ) ? 32 : 0; //DOWN-LEFT

        mask += HasWallTile( tilemap, position + new Vector3Int( 0, -1, 0 ) ) ? 64 : 0; //DOWN

        mask += HasWallTile( tilemap, position + new Vector3Int( 1, -1, 0 ), BOTTOM_RIGHT ) ? 128 : 0; //DOWN RIGHT

        int index = GetIndex((byte) mask);

        if( index >= 0 && index < sprites.Length ) {
            ;
            tileData.sprite = sprites[index];
            tileData.color = Color.white;
            tileData.transform.SetTRS( Vector3.zero, Quaternion.identity, Vector3.one );
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.None;
        } else {
            Debug.LogWarning( "Not enough sprites in EightBitTile instance. -- " + position + ": " + mask + " : " + index );
        }
    }

    private int GetIndex( byte mask ) {
        if( indexDictionary.ContainsKey( mask ) ) {
            return indexDictionary[mask];
        }
        return -1;
    }

    private bool HasWallTile( ITilemap tilemap, Vector3Int position ) {
        return tilemap.GetTile( position ) == this;
    }

    private bool HasWallTile( ITilemap tilemap, Vector3Int position, Vector3Int corner ) {
        Vector3Int centerTile = position - corner;

        //
        if( tilemap.GetTile( position ) ) {
            if( corner == TOP_LEFT ) {
                return ( tilemap.GetTile( centerTile + TOP ) && tilemap.GetTile( centerTile + LEFT ) );
            }
            if( corner == TOP_RIGHT ) {
                return ( tilemap.GetTile( centerTile + TOP ) && tilemap.GetTile( centerTile + RIGHT ) );
            }
            if( corner == BOTTOM_LEFT ) {
                return ( tilemap.GetTile( centerTile + BOTTOM ) && tilemap.GetTile( centerTile + LEFT ) );
            }
            if( corner == BOTTOM_RIGHT ) {
                return ( tilemap.GetTile( centerTile + BOTTOM ) && tilemap.GetTile( centerTile + RIGHT ) );
            }
        }

        return false;
    }

    private bool HasCardinalNeighbor( ITilemap tilemap, Vector3Int position ) {
        return ( ( tilemap.GetTile( position + TOP ) || tilemap.GetTile( position + BOTTOM ) || tilemap.GetTile( position + LEFT ) || tilemap.GetTile( position + RIGHT ) ) == this );
    }

#if UNITY_EDITOR

    [MenuItem( "Tile Assets/Create/8-Bit Tile" )]
    public static void CreateWallTile() {
        string path = EditorUtility.SaveFilePanelInProject("Save 8-Bit Tile", "New 8-Bit Tile", "asset", "Save 8-Bit Tile", "Assets");
        if( path == "" ) {
            return;
        }
        AssetDatabase.CreateAsset( ScriptableObject.CreateInstance<EightBitTile>(), path );
    }

#endif
}