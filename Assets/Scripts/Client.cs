﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public string clientName;
    public bool isHost;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private List<GameClient> players = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket Error:" + e.Message);
        }
        return socketReady;

    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }
    }
    //Send message to server
    public void Send(string data)
    {
        if (!socketReady)
            return;
        writer.WriteLine(data);
        writer.Flush();

    }

    //Read Message from Server
    private void OnIncomingData(string data)
    {
        Debug.Log("client:" + data);
        var aData = data.Split('|');
        switch (aData[0])
        {
            case "SWHO":
                for (int i = 1; i < aData.Length - 1; i++)
                {
                    UserConnected(aData[i], false);
                }
                Send("CWHO|" + clientName + "|" + ((isHost) ? 1 : 0).ToString());
                break;
            case "SCNN":
                UserConnected(aData[1], false);
                break;
            case "SOVE":
                Field.Instance.Move(aData[1], int.Parse(aData[2]));
                break;
            case "SRAW":
                Field.Instance.Draw(aData[1]);
                break;

        }
    }
    private void UserConnected(string name, bool host)
    {
        GameClient c = new GameClient();
        c.name = name;

        players.Add(c);

        if (players.Count == 2)
        {
            GameManager.Instance.StartGame();
        }


    }
    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
        {
            return;
        }
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}


public class GameClient
{
    public string name;
    public bool isHost;
}
