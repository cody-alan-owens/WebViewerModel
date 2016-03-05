using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Model
{
    class ModelSceneWebSocketClient : MonoBehaviour
    {
		public UI_Control_Quick_905Utah quickOptions; 
		public SocketRunner SocketRunner;
		public string Server;
		public int Port;
		public string ID;
		public string ModelRID;
		void Start()
        {
			this.ID = getIDArg()+ModelRID;
			SocketRunner = new SocketRunner(this, Server, Port, ID);
        }

		void Update(){
			if(this.SocketRunner.Updated){
				SocketRunner.Updated = false;
				sceneUpdate(SocketRunner.LastJSON);
			}
		}
			
		void OnApplicationQuit(){
			Debug.Log("Ending server listening thread...");
			SocketRunner.listening = false;
			SocketRunner.ClientSocket.Close();
			if(SocketRunner.serverListeningThread.IsAlive){
				SocketRunner.serverListeningThread.Abort();
			}
		}

        
		public void sceneUpdate(string json){
			Debug.Log("Updating scene to the following: "+json);
			Dictionary<string,string> config = JsonConvert.DeserializeObject<Dictionary<string,string>>(json);
			quickOptions.setConfig(config);
		}

		private string getIDArg(){
			if(String.IsNullOrEmpty(ID)){
				String[] arguments = System.Environment.GetCommandLineArgs();
				Debug.Log("Found "+arguments.Length+" arguments.");
				if (arguments.Length > 1) {
					Debug.Log("Arguments: "+arguments[1]);
					return arguments[1];
				}
			}
			return ID;
		}
    }   

	class SocketRunner{
		ModelSceneWebSocketClient SocketClient;
		public Thread serverListeningThread;
		public bool Updated = false;
		public string LastJSON = String.Empty;
		public string Server;
		public int Port;
		public string ID;
		public bool listening = true;
		public TcpClient ClientSocket;
		public SocketRunner(ModelSceneWebSocketClient socketClient, string server, int port, string id){
			this.SocketClient = socketClient;
			this.Server = server;
			this.Port = port;
			this.ID = id;
			serverListeningThread = new Thread(listenToServer);
			serverListeningThread.IsBackground = true;
			serverListeningThread.Start();
		}
		public void listenToServer()
		{
			try
			{
				ClientSocket = new TcpClient(Server, Port);

				NetworkStream serverStream = ClientSocket.GetStream();
				byte[] outStream = System.Text.Encoding.ASCII.GetBytes(ID + "$");
				serverStream.Write(outStream, 0, outStream.Length);
				serverStream.Flush();

				byte[] inStream = new byte[10025];
				this.ClientSocket.ReceiveBufferSize = inStream.Length;
				while (listening)
				{
					try{						
						serverStream.Read(inStream, 0, ClientSocket.ReceiveBufferSize);
						string returndata = System.Text.Encoding.ASCII.GetString(inStream);
						inStream = new byte[10025];
						Debug.Log("Data from Server : " + returndata);
						if (returndata.IndexOf("id") != -1)
						{
							this.LastJSON = returndata;
							this.Updated = true;
						}
					} catch(System.IO.IOException e){
						Debug.Log("Connection to server lost.");
						break;
					}
				}
			}
			catch (Exception e)
			{
				Debug.Log("Error in socket script.");
			}
		}
	}
}
