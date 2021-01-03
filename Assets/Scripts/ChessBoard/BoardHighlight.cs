using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlight : MonoBehaviour
{
    public GameObject selectionHighlihgtPrefab;
    public GameObject moveHighlightPrefab;
    [Space]
    public float selectionHighlightHeight = 0.015f;
    public float moveHighlightHeight = 0.025f;

    private GameObject _selectionHighlight;
    private GameObject[,] _moveHighlights;

    void Start()
    {
        CreateSelectionHighlight();
        CreateMoveHighligts();

        EventManager<Vector2Int, bool>.AddEvent("UpdateSelectionHighlight", UpdateSelectionHighlight);
        EventManager<bool[,]>.AddEvent("EnableMoveHighligts", EnableMoveHighligts);
        EventManager.AddEvent("DisableMoveHighligts", DisableMoveHighligts);
    }

    #region Create selection and moves highlights

    private void CreateSelectionHighlight()
    {
        _selectionHighlight = Instantiate(selectionHighlihgtPrefab, new Vector3(0f, selectionHighlightHeight, 0f), Quaternion.identity);
    }

    private void CreateMoveHighligts()
    {
        _moveHighlights = new GameObject[8, 8];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j] = Instantiate(moveHighlightPrefab, new Vector3(i, moveHighlightHeight, j), Quaternion.identity);
                _moveHighlights[i, j].SetActive(false);
            }
        }
    }

    #endregion

    #region Update highlights

    private void UpdateSelectionHighlight(Vector2Int position, bool isActive)
    {
        _selectionHighlight.SetActive(isActive);

        if (isActive)
        {
            _selectionHighlight.transform.position = new Vector3(position.x, selectionHighlightHeight, position.y);
        }
    }

    private void EnableMoveHighligts(bool[,] allowedMoves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j].SetActive(allowedMoves[i, j]);
            }
        }
    }

    private void DisableMoveHighligts()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j].SetActive(false);
            }
        }
    }

    #endregion
}
