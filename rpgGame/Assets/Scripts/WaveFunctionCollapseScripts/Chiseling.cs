﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chiseling : MonoBehaviour
{
    GridManager gridManager;
    public slot[,,] grid;
    List<slot> fixedPoints = new List<slot>();
    List<slot> allPoints = new List<slot>();
    List<slot> visitedPoints = new List<slot>();
    public List<Vector2> fixedPointsPositions = new List<Vector2>();
    int tries = 0;
    int size = 0;
    int progress;
    int fixedSlotsVisited;

    private void Awake()
    {

    }

    public void AssignFixedPoints()
    {
        gridManager = GetComponent<GridManager>();
        size = gridManager.gridX * gridManager.gridZ;

        for (int i = 0; i < gridManager.gridX; i++)
        {
            for (int k = 0; k < gridManager.gridY; k++)
            {
                for (int j = 0; j < gridManager.gridZ; j++)
                {
                    allPoints.Add(grid[i, k, j]);
                    grid[i, k, j].isFixed = false;
                }
            }
        }

        for (int i = 0; i < fixedPointsPositions.Count; i++)
        {
            fixedPoints.Add(grid[(int)fixedPointsPositions[i].x, 0, (int)fixedPointsPositions[i].y]);
            grid[(int)fixedPointsPositions[i].x, 0, (int)fixedPointsPositions[i].y].isFixed = true;
        }
        StartCoroutine(TryToRemove());
    }

    public IEnumerator TryToRemove()
    {
        int itemToRemove = Random.Range(0, allPoints.Count);

        allPoints[itemToRemove].isPath = false;

        if (CheckFixedPoints())
        {
            tries = 0;
            progress++;
            allPoints.RemoveAt(itemToRemove);
        }
        else
        {
            tries++;
            allPoints[itemToRemove].isPath = true;
        }

        if (tries > 100)
        {
            gridManager.startBuilding();
            //gridManager.Build();
        }
        else
        {
            Reset();
            yield return null;
            StartCoroutine(TryToRemove());
        }
    }



    public IEnumerator Visit(slot slot)
    {
        if (fixedSlotsVisited == fixedPoints.Count)
        {
            yield return null;
        }
        else
        {
            if (slot.isPath && !slot.isVisited)
            {
                slot.isVisited = true;
                visitedPoints.Add(slot);

                if (slot.isFixed)
                {
                    fixedSlotsVisited++;
                }

                if (slot.neighbours[0] != null)
                    StartCoroutine(Visit(slot.neighbours[0]));
                if (slot.neighbours[1] != null)
                    StartCoroutine(Visit(slot.neighbours[1]));
                if (slot.neighbours[2] != null)
                    StartCoroutine(Visit(slot.neighbours[2]));
                if (slot.neighbours[3] != null)
                    StartCoroutine(Visit(slot.neighbours[3]));
            }
        }


    }

    private void Reset()
    {
        //Debug.Log(visitedPoints.Count + " and " + fixedSlotsVisited + " and " + fixedPoints.Count);
        for (int i = 0; i < visitedPoints.Count; i++)
        {
            visitedPoints[i].isVisited = false;
        }
        visitedPoints.Clear();
    }

    private bool CheckFixedPoints()
    {
        fixedSlotsVisited = 0;
        StartCoroutine(Visit(fixedPoints[0]));
        for (int i = 0; i < fixedPoints.Count; i++)
        {
            if (!fixedPoints[i].isVisited)
            {
                return false;
            }
        }
        //Debug.Log(visitedPoints.Count + " and " + fixedSlotsVisited + " and " + fixedPoints.Count);
        return true;
    }
}
