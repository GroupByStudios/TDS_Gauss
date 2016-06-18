using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MouseCursor : MonoBehaviour
{
    private Image _crossHairImage;

    [HideInInspector]
    public Vector3 WorldCursorPosition;
    [HideInInspector]
    public Vector2 ScreenSpaceCursorPosition;

    // Player who is controlling the Mouse Input
    Player _player = null;

    void Start()
    {
        _crossHairImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;

        if (_crossHairImage != null)
        {
            _player = PlayerManager.Instance.ActivePlayers.Find(p => p.PlayerInputController != null && p.PlayerInputController.IsKeyboardAndMouse);

            if (_player != null)
            {
                _crossHairImage.enabled = true;
                _crossHairImage.color = PlayerManager.Instance.myPlayerColorList[(int)_player.PlayerClass];

                Vector3 laserEndMousePosition = Vector3.zero;
                Vector3 laserEndDirection = _player.LaserEnd - _player.transform.position;

                float scaleX = _player.MouseLookPosition.x / laserEndDirection.x;
                float scaleY = _player.MouseLookPosition.y / laserEndDirection.y;
                float scaleZ = _player.MouseLookPosition.z / laserEndDirection.z;

                laserEndMousePosition.x = laserEndDirection.x * scaleX;
                laserEndMousePosition.y = laserEndDirection.y * scaleY;
                laserEndMousePosition.z = laserEndDirection.z * scaleZ;

                WorldCursorPosition = _player.LaserOrigin + laserEndMousePosition;
                WorldCursorPosition.y = _player.LaserOrigin.y;

                ScreenSpaceCursorPosition = Camera.main.WorldToScreenPoint(WorldCursorPosition);
                transform.position = new Vector3(ScreenSpaceCursorPosition.x, ScreenSpaceCursorPosition.y, 0);
            }
            else
            {
                _crossHairImage.enabled = false;
            }

        }
    }
}
