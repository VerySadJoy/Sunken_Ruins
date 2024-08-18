using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class MapArray : MonoBehaviour
{
    int[,] baseMap; int[,] realMap;
    int startX; int startY;
    int endX; int endY;
    int tileTraceX = 10; int tileTraceY = 10;

    // Tile Boundaries
    int startTileX = 10; int startTileY = 10;
    // int endTileX; int endTileY;
    int mapXLength = 10; int mapYLength = 12;

    [SerializeField] private int mapHeight = 8;
    [SerializeField] private int mapWidth = 8;

    // Tiles
    [SerializeField] GameObject dwellingTile;

    // Enemies
    [SerializeField] GameObject ElectricStingRay;
    [SerializeField] GameObject HypnoCuttleFish;
    [SerializeField] GameObject AngryShell;
    [SerializeField] GameObject ThrowingCrab;    

    enum RoomType
    {
        RandomRoom = 0,
        UpRoom = 1,
        RightRoom = 2,
        DownRoom = 3,
        LeftRoom = 4,
        UpRightRoom = 5,
        DownRightRoom = 6,
        DownLeftRoom = 7,
        UpLeftRoom = 8,
        UpDownRoom = 9,
        LeftRightRoom = 10,
        UpDownRightRoom = 11,
        UpDownLeftRoom = 12,
        DownLeftRightRoom = 13,
        UpLeftRightRoom = 14,
        AllSidesRoom = 15,
        StartRoom = 16,
        EndRoom = 17,
    }

    enum TileType
    {
        Blank = 0,
        Block = 1,
        Wood = 9,
        ElectricStingRay = 10,
        HypnoCuttleFish = 11,
        AngryShell = 12,
        ThrowingCrab = 13,
    }

    // Size of One Map: 10 * 12
    int[,,] StartRoom = new int[2, 12, 10]
    {
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 0, 0, 0, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        },

        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        },
    };

    int[,,] EndRoom = new int[1, 12, 10]
    {
        {
            { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        },
    };

    int[,,] RandomRoom = new int[12, 12, 10]
    {
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 0, 0, 1, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 0, 0, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 1, 1, 0, 1, 1 },
            { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
        },

        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 1, 1, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 0, 0, 0, 0, 1, 1, 1, 1},
        },

        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 0, 0, 1, 1},
            {1, 1, 1, 1, 1, 0, 0, 0, 0, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 1, 1, 1, 1},
        },

        {
            {1, 1, 1, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 0, 0, 0, 1, 1, 1, 1, 1, 1},
            {1, 0, 0, 0, 0, 1, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        },
    
        {
            {1, 1, 1, 1, 1, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 1, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 1, 0, 0, 1, 0, 0, 1},
            {1, 1, 1, 1, 0, 0, 1, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 1, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 1, 1, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 1, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        },
    
        {
            {1, 1, 1, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 1, 0, 0, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        },

        // 아래 위 뚫리는 방
        {
            {1, 1, 1, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 1, 1, 1},
        },
    
        {
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 1, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 1, 0, 0, 1, 0, 0, 1},
            {1, 1, 1, 1, 0, 0, 1, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 1, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 1, 1, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
        },
    
        {
            {1, 1, 1, 1, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 0, 0, 0, 0, 1, 1, 1, 1},
        }
    };
    
    int[,,] LeftRightRoom = new int[2, 12, 10]{
        {
            { 1, 1, 1, 0, 0, 0, 0, 1, 1, 1 },
            { 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 0, 0, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        },

        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        },
    };

    int[,,] UpLeftRightRoom = new int[3, 12, 10]{
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },
    };

    int[,,] DownLeftRightRoom = new int[3, 12, 10]{
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 1, 1, 1, 1, 0, 1, 1 },
            { 0, 0, 0, 0, 1, 1, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        },

        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 0, 1, 1, 1, 0, 0, 1 },
            { 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 },
        },

        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 0, 1, 1, 0, 1 },
            { 1, 0, 1, 1, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 },
            { 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        },
    };

    int[,,] UpLeftRoom = new int[3, 12, 10]{
        {
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
            { 0, 1, 1, 0, 0, 0, 0, 1, 1, 1 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 1, 1, 1, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
            { 0, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 1 },
            { 1, 1, 0, 0, 1, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 1, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        }
    };

    int[,,] UpRightRoom = new int[3, 12, 10]{
        {
            { 1, 1, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 1, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },

        {
            { 1, 0, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 1, 0, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 0, 1, 1, 0, 0 },
            { 1, 1, 1, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        },
    };

    int[,,] DownLeftRoom = new int[3, 12, 10]{
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {0, 1, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 1},
        },

        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 1, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {0, 0, 0, 0, 1, 1, 1, 0, 0, 1},
            {0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
            {0, 1, 0, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
        },

        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 0, 0, 1, 1, 1, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 1, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
            {0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 1, 0, 0, 0, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 1, 1},
        }
    };

    int[,,] DownRightRoom = new int[3, 12, 10]{
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 0, 0, 1, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 0},
            {1, 1, 1, 1, 0, 0, 0, 1, 1, 0},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 1, 1, 1, 1},
        },

        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 0, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 1, 1, 0, 0, 0, 0},
            {1, 0, 1, 1, 1, 0, 0, 0, 0, 0},
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 0},
            {1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 1, 1, 1, 1},
        },

        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 0, 0, 0, 0, 1, 0, 0},
            {1, 1, 1, 0, 0, 0, 0, 1, 1, 1},
            {1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
        }
    };

    int[,,] AllSidesRoom = new int[2, 12, 10]{
        {
            { 1, 1, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1 },
        },

        {
            { 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 1, 0, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1 },
        },
    };

    private void Start()
    {
        // endTileX = startTileX + 4 * mapXLength;
        // endTileY = startTileY - 4 * mapYLength;

        MakeRealMap();
    }

    private void MakeRealMap()
    {
        MakeBaseMap();

        realMap = new int[mapHeight * mapYLength, mapWidth * mapXLength]; // 맵 크기 바꿀 때마다 숫자 바꿔야 함
        int realStartX = 0; int realStartY = 0;
        int realTraceX; int realTraceY;
        int roomNumber;
        for (int y = 0; y < baseMap.GetLength(0); ++y)
        {
            for (int x = 0; x < baseMap.GetLength(1); ++x)
            {
                realTraceX = realStartX;
                realTraceY = realStartY;

                switch ((RoomType)baseMap[y, x])
                {
                    case RoomType.RandomRoom:
                        roomNumber = Random.Range(0, RandomRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = RandomRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.UpRoom:
                    break;

                    case RoomType.RightRoom:
                    break;

                    case RoomType.DownRoom:
                    break;

                    case RoomType.LeftRoom:
                    break;

                    case RoomType.UpRightRoom:
                        roomNumber = Random.Range(0, UpRightRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = UpRightRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.DownRightRoom:
                        roomNumber = Random.Range(0, DownRightRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = DownRightRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.DownLeftRoom:
                        roomNumber = Random.Range(0, DownLeftRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = DownLeftRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.UpLeftRoom:
                        roomNumber = Random.Range(0, UpLeftRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = UpLeftRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.UpDownRoom:
                    break;

                    case RoomType.LeftRightRoom:
                        roomNumber = Random.Range(0, LeftRightRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = LeftRightRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.UpDownRightRoom:
                    break;

                    case RoomType.UpDownLeftRoom:
                    break;

                    case RoomType.DownLeftRightRoom:
                        roomNumber = Random.Range(0, DownLeftRightRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = DownLeftRightRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.UpLeftRightRoom:
                        roomNumber = Random.Range(0, UpLeftRightRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = UpLeftRightRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.AllSidesRoom:
                        roomNumber = Random.Range(0, AllSidesRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = AllSidesRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.StartRoom:
                        roomNumber = Random.Range(0, StartRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = StartRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    case RoomType.EndRoom:
                        roomNumber = Random.Range(0, EndRoom.GetLength(0));
                        for (int roomY = 0; roomY < mapYLength; ++roomY)
                        {
                            for (int roomX = 0; roomX < mapXLength; ++roomX)
                            {
                                realMap[realTraceY, realTraceX++] = EndRoom[roomNumber, roomY, roomX];
                            }
                            realTraceX = realStartX;
                            ++realTraceY;
                        }
                        realStartX += mapXLength;
                        break;

                    default:
                        Debug.LogError("RoomType DNE Error");
                        break;
                }

                if (realStartX >= mapWidth * mapXLength)
                {
                    realStartX = 0; realStartY += mapYLength; // �� ���� y�� ���� = 8
                }
            }
        }

        CheckMonsterGenerationByTile(); // change int[] array instead of using collision detection
        TilePlacement(realMap);
    }

    private void MakeBaseMap()
    {
        baseMap = new int[mapHeight, mapWidth];
        startX = Random.Range(0, mapWidth);
        startY = 0;
        baseMap[startY, startX] = (int)RoomType.StartRoom;

        int basetraceX = startX; int basetraceY = startY;
        int moveProb;
        do moveProb = Random.Range(0, 3); while (moveProb == 1);
        while (endY != mapHeight - 1)
        {
            switch (moveProb)
            {
                // Move Left
                case 0:
                    // Doesn't Get out of Bounds
                    if (basetraceX - 1 >= 0)
                    {
                        // if (baseMap[basetraceY, basetraceX - 1] != (int)RoomType.RandomRoom)
                        // {
                        //     moveProb = Random.Range(1, 3);
                        //     break;
                        // }

                        baseMap[basetraceY, --basetraceX] = (int)RoomType.LeftRightRoom;
                        moveProb = Random.Range(0, 2);
                    }
                    else
                    {
                        if (basetraceX + 1 < mapWidth && baseMap[basetraceY, basetraceX + 1] == (int)RoomType.RandomRoom) moveProb = 2;
                        else moveProb = 1;
                    }
                    break;

                // Move Down
                case 1:
                    if (baseMap[basetraceY, basetraceX] == (int)RoomType.StartRoom)
                    {
                        do moveProb = Random.Range(0, 3); while (moveProb == 1);
                    }
                    else if (basetraceY + 1 <= mapHeight - 1)
                    {
                        // Determine whether next move is left or right
                        do moveProb = Random.Range(0, 3); while (moveProb == 1);

                        // Check for Room Open-ness
                        if (basetraceX - 1 < 0) // if left is void
                        {
                            baseMap[basetraceY, basetraceX] = (int)RoomType.DownRightRoom;
                            baseMap[++basetraceY, basetraceX] = (int)RoomType.UpRightRoom;
                        }
                        else if (basetraceX + 1 >= mapWidth) // if right is void
                        {
                            baseMap[basetraceY, basetraceX] = (int)RoomType.DownLeftRoom;
                            baseMap[++basetraceY, basetraceX] = (int)RoomType.UpLeftRoom;
                        }
                        else 
                        {
                            // Prob of having room with 3 open sides = 20%
                            bool isThreeSidesOpen = Random.Range(0, 10) < 2;
                            if (isThreeSidesOpen)
                            {
                                baseMap[basetraceY, basetraceX] = (int)RoomType.DownLeftRightRoom;
                                baseMap[++basetraceY, basetraceX] = (int)RoomType.UpLeftRightRoom;
                            }
                            else // open only two sides depending on next move
                            {
                                // Check for Above Room
                                if (baseMap[basetraceY, basetraceX - 1] != (int)RoomType.RandomRoom) // If Left is NOT a RandomRoom,
                                {
                                    baseMap[basetraceY, basetraceX] = (int)RoomType.DownLeftRoom;
                                }
                                else if (baseMap[basetraceY, basetraceX + 1] != (int)RoomType.RandomRoom) // If Right is NOT a RandomRoom,
                                {
                                    baseMap[basetraceY, basetraceX] = (int)RoomType.DownRightRoom;
                                }
                                else
                                {
                                    Debug.LogError("Error: Both Sides are NOT void and NOT randomRooms at the same time");
                                }

                                // Check for Below Room
                                if (moveProb == 0) // next move = LEFT
                                {
                                    baseMap[++basetraceY, basetraceX] = (int)RoomType.UpLeftRoom;
                                }
                                else if (moveProb == 2) // next move = RIGHT
                                {
                                    baseMap[++basetraceY, basetraceX] = (int)RoomType.UpRightRoom;
                                }
                                else Debug.LogError("Next Move Error with MapGen");
                            }
                        }
                    }
                    // Reached EndPoint
                    else
                    {
                        endX = basetraceX; endY = basetraceY;
                        baseMap[endY, endX] = (int)RoomType.EndRoom;
                    }
                    break;

                // Move Up
                

                // Move Right
                case 2:
                    // Doesn't Get out of Bounds
                    if (basetraceX + 1 < mapWidth)
                    {
                        // if (baseMap[basetraceY, basetraceX + 1] != (int)RoomType.RandomRoom)
                        // {
                        //     moveProb = Random.Range(0, 2);
                        //     break;
                        // }

                        baseMap[basetraceY, ++basetraceX] = (int)RoomType.LeftRightRoom;
                        moveProb = Random.Range(1, 3);
                    }
                    else
                    {
                        if (basetraceX - 1 >= 0 && baseMap[basetraceY, basetraceX - 1] == (int)RoomType.RandomRoom) moveProb = 0;
                        else moveProb = 1;
                    }
                    break;

                default:
                    Debug.LogError("Error with Map 2DArray Generation");
                    break;
            }
        }

        TestBaseMapGeneration();

        // For maps with consecutive "DownLeftRight" rooms vertically
        // for (int y = 1; y < baseMap.GetLength(0); ++y)
        // {
        //     if (baseMap[y - 1, 0] == 3 && baseMap[y, 0] == 3)
        //     {
        //         baseMap[y - 1, 0] = (int)RoomType.AllSides;
        //         baseMap[y, 0] = (int)RoomType.AllSides;
        //     }
        //     else if (baseMap[y - 1, 1] == 3 && baseMap[y, 1] == 3)
        //     {
        //         baseMap[y - 1, 0] = (int)RoomType.AllSides;
        //         baseMap[y, 0] = (int)RoomType.AllSides;
        //     }
        //     else if (baseMap[y - 1, 2] == 3 && baseMap[y, 2] == 3)
        //     {
        //         baseMap[y - 1, 0] = (int)RoomType.AllSides;
        //         baseMap[y, 0] = (int)RoomType.AllSides;
        //     }
        //     else if (baseMap[y - 1, 3] == 3 && baseMap[y, 3] == 3)
        //     {
        //         baseMap[y - 1, 0] = (int)RoomType.AllSides;
        //         baseMap[y, 0] = (int)RoomType.AllSides;
        //     }
        //     else continue;
        // }
        // TestBaseMapGeneration();
    }

    private void TilePlacement(int[,] realMap)
    {
        for (int y = 0; y < realMap.GetLength(0); ++y)
        {
            for (int x = 0; x < realMap.GetLength(1); ++x)
            {
                switch ((TileType)realMap[y, x])
                {
                    case TileType.Blank:
                        tileTraceX += 2;
                        break;

                    case TileType.Block:
                        Instantiate(dwellingTile, new Vector3(tileTraceX, tileTraceY), Quaternion.identity);
                        tileTraceX += 2;
                        break;

                    case TileType.Wood:
                        Instantiate(dwellingTile, new Vector3(tileTraceX, tileTraceY), Quaternion.identity);
                        tileTraceX += 2;
                        break;

                    case TileType.ElectricStingRay:
                        Instantiate(ElectricStingRay, new Vector3(tileTraceX, tileTraceY), Quaternion.identity);
                        tileTraceX += 2;
                        break;

                    case TileType.HypnoCuttleFish:
                        Instantiate(HypnoCuttleFish, new Vector3(tileTraceX, tileTraceY), Quaternion.identity);
                        tileTraceX += 2;
                        break;

                    case TileType.AngryShell:
                        Instantiate(AngryShell, new Vector3(tileTraceX, tileTraceY), Quaternion.identity);
                        tileTraceX += 2;
                        break;
                    
                    case TileType.ThrowingCrab:
                        Instantiate(ThrowingCrab, new Vector3(tileTraceX, tileTraceY), Quaternion.identity);
                        tileTraceX += 2;
                        break;

                    default:
                        Debug.LogError("Tile Type DNE Error");
                        break;
                }
            }
            tileTraceX = startTileX; tileTraceY -= 2;
        }
    }

    private void CheckMonsterGenerationByTile()
    {
        for (int y = 1; y < realMap.GetLength(0) - 1; ++y)
        {
            for (int x = 1; x < realMap.GetLength(1) - 1; ++x)
            {
                // Check Boundaries
                bool isUpBlank = realMap[y + 1, x] == 0;
                bool isDownBlank = realMap[y - 1, x] == 0;
                bool isLeftBlank = realMap[y, x - 1] == 0;
                bool isRightBlank = realMap[y, x + 1] == 0;

                // Electric StingRay & HypnoCuttleFish
                if (isUpBlank && isDownBlank && isLeftBlank && isRightBlank)
                {
                    // Summon Probability = 20%
                    bool isInstantiate = Random.Range(0, 100) < 2;
                    if (isInstantiate)
                    {
                        realMap[y, x] = (int)TileType.ElectricStingRay;
                    }
                }

                // AngryShell
                if (isUpBlank && isBlank(y, 2, x, 0) && isLeftBlank && isRightBlank && !isDownBlank)
                {
                    // Summon Probability = 20%
                    bool isInstantiate = Random.Range(0, 100) < 2;
                    if (isInstantiate)
                    {
                        realMap[y, x] = (int)TileType.AngryShell;
                    }
                }

                // Throwing Crab


            }
        }
    }

    private bool isBlank(int y, int changeY, int x, int changeX)
    {
        if (y + changeY >= 0 && y + changeY < realMap.GetLength(0))
        {
            if (x + changeX >= 0 && x + changeX < realMap.GetLength(1))
            {
                return realMap[y + changeY, x + changeX] == 0;
            }
        }

        // IndexOutOfBounds
        return true;
    }

    private void TestBaseMapGeneration()
    {
        string row = null;
        for (int i = 0; i < baseMap.GetLength(0); ++i)
        {
            for (int j = 0; j < baseMap.GetLength(1); ++j)
            {
                row += baseMap[i, j].ToString() + " ";
            }
            Debug.Log(row);
            row = null;
        }
    }
}