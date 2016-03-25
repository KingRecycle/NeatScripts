using System.Collections;
using UnityEngine;

public class PointerGrid : MonoBehaviour {

    [SerializeField]
    private Vector3 grid;

    [SerializeField]
    public Vector3 Grid
    {
        get { return grid; }
        private set { grid = value; }
    }

    public Camera cam;
    public GameObject gridIndicator = null;
    public bool showIndicator;
    public bool alignByCenter;
    public int zOffsetFromCamera;

    private Transform indicatorClone = null;

    private void Update() {
        if ( cam == null ) { return; }

        Vector3 wp = cam.ScreenToWorldPoint( Input.mousePosition );

        if ( showIndicator ) {
            if ( gridIndicator == null ) {
                return;
            }
            if ( indicatorClone == null ) {
                indicatorClone = Instantiate( gridIndicator ).transform;
            } else {
                if ( alignByCenter ) {
                    indicatorClone.position = grid;
                } else {
                    indicatorClone.position = new Vector3( grid.x += 0.5f, grid.y += 0.5f, grid.z );
                }
            }
        } else {
            if ( indicatorClone ) {
                Destroy( indicatorClone.gameObject );
            }
        }

        grid = new Vector3( Mathf.Floor( wp.x ), Mathf.Floor( wp.y ), Mathf.Floor( wp.z + zOffsetFromCamera ) );
    }
}