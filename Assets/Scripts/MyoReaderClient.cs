using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using TMPro;

#if !UNITY_EDITOR
using System.Threading.Tasks;
#else 
using System.Threading;
#endif

public class MyoReaderClient : MonoBehaviour {

#if !UNITY_EDITOR
    private Windows.Networking.Sockets.StreamSocket socket;
    private Task socketListenTask;
#endif

    private int port = 8099;
    private string host = "127.0.0.1";
    private string ipAddress = "x.x.x.x"; // Replace with computer IP
    private string portUWP = "8099";
    public string control = "Starting!";
    public int consecutive = 0;
    private StreamReader reader;
    private bool connected = false;


#if UNITY_EDITOR
    TcpClient socketClient;
#endif

    void Start () {
#if !UNITY_EDITOR
        ConnectSocketUWP();
#else
        ConnectSocketUnity();
        var readThread = new Thread(new ThreadStart(ListenForDataUnity));
        readThread.IsBackground = true;
        readThread.Start();
#endif
    }
    
    void Update () {
#if !UNITY_EDITOR
        if(socketListenTask == null || socketListenTask.IsCompleted)
        {
            socketListenTask = new Task(async() =>{ListenForDataUWP();});
            socketListenTask.Start();
        }
#endif
    }
    
#if UNITY_EDITOR
    void ConnectSocketUnity()
    {
        IPAddress ipAddress = IPAddress.Parse(host);

        socketClient = new TcpClient();
        try
        {
            socketClient.Connect(ipAddress, port);
        }

        catch
        {
            Debug.Log("error when connecting to server socket");
        }
    }
#else
    private async void ConnectSocketUWP()
    {
        try
        {
            socket = new Windows.Networking.Sockets.StreamSocket();
            Windows.Networking.HostName serverHost = new Windows.Networking.HostName(ipAddress);
            await socket.ConnectAsync(serverHost, portUWP);
            Stream streamIn = socket.InputStream.AsStreamForRead();
            reader = new StreamReader(streamIn, Encoding.UTF8);
            connected = true;
            control = "Connected";
        }
        catch (Exception e)
        {
            control = "Connection Error";
        }
    }
#endif

#if UNITY_EDITOR
    void ListenForDataUnity()
    {
        int data;
        while(true){
            byte[] bytes = new byte[socketClient.ReceiveBufferSize];
            NetworkStream stream = socketClient.GetStream();
            data = stream.Read(bytes, 0, socketClient.ReceiveBufferSize);
            string tControl = Encoding.UTF8.GetString(bytes, 0, data).Trim();
            // Keep track of consecutive agreements
            if (tControl == control) {
                consecutive += 1; 
            } else {
                consecutive = 0;
            }
            control = tControl;

        }
    }
#else
    private void ListenForDataUWP()
    {
        try {
            // Keep track of consecutive agreements
            string tControl = reader.ReadLine().Trim();
            if (tControl == control) {
                consecutive += 1; 
            } else {
                consecutive = 0;
            }
            control = tControl;
        } catch (Exception e) {
            //Do nothing
        }
    }
#endif
}
