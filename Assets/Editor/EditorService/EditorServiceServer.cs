
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace EditorService
{
    sealed class Server : ScriptableSingleton<Server>, ISerializationCallbackReceiver
    {
        void Awake()
        {
            Debug.Print("Server::Awake");

            hideFlags = HideFlags.DontSave;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Debug.Print("Server::OnAfterDeserialize");

            if (_listener == null) start();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            Debug.Print("Server::OnBeforeSerialize");

            if (_listener != null) stop();
        }

        private void start()
        {
            Debug.Print("Server::start");

            Assert.IsNull(_listener);

            try
            {
                _lock = new object();
                _queue = new Queue<HttpListenerContext>();
                EditorApplication.update += onUpdate;

                _listener = new HttpListener();
                _listener.Prefixes.Add(@"http://*:8889/");
                _listener.Start();
                _listener.BeginGetContext(onRequest, _listener);
            }
            catch (Exception e)
            {
                Debug.Exception(e);
            }
        }

        private void stop()
        {
            Debug.Print("Server::stop");

            Assert.IsNotNull(_listener);

            var temp = _listener;
            _listener = null;
            temp.Close();

            EditorApplication.update -= onUpdate;
            _queue = null;
            _lock = null;
        }

        private void respond(HttpListenerContext context)
        {
            Assert.IsNotNull(context);

            var req = context.Request;
            Debug.PrintFormat("Server::respond(\"{0}\")", req.Url.AbsolutePath);

            using (var res = context.Response)
            using (var output = new StreamWriter(res.OutputStream))
            {
                switch (req.Url.AbsolutePath)
                {
                    case "/":
                    case "/Version":
                        res.StatusCode = (int)HttpStatusCode.OK;
                        output.Write(Application.unityVersion);
                        output.Flush();
                        break;

                    case "/HasError":
                        res.StatusCode = (int)HttpStatusCode.OK;
                        output.Write(CompileErrorChecker.HasError() ? "true" : "false");
                        output.Flush();
                        break;

                    case "/Compile":
                        CompileErrorChecker.ForceCompile();
                        res.StatusCode = (int)HttpStatusCode.OK;
                        output.Write("Processed");
                        output.Flush();
                        break;

                    default:
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                }
            }
        }

        private void onUpdate()
        {
            if (_queue.Count > 0)
            {
                lock (_lock)
                {
                    while (_queue.Count > 0)
                    {
                        try
                        {
                            respond(_queue.Dequeue());
                        }
                        catch (Exception e)
                        {
                            Debug.Exception(e);
                        }
                    }
                }
            }
        }

        private void onRequest(IAsyncResult result)
        {
            Debug.Print("Server::onRequest");

            Assert.IsNotNull(result);
            var listener = result.AsyncState as HttpListener;
            Assert.IsNotNull(listener);
            if (!listener.IsListening)
            {
                Debug.Print("Finish Listening");
                return;
            }

            try
            {
                var context = listener.EndGetContext(result);
                lock (_lock)
                {
                    _queue.Enqueue(context);
                }
                listener.BeginGetContext(onRequest, listener);
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException && _listener == null)
                {
                    Debug.Print("Closed httpListener");
                }
                else
                {
                    Debug.Exception(e);
                }
            }
        }

        private object _lock = null;
        private HttpListener _listener = null;
        private Queue<HttpListenerContext> _queue = null;
    }
}
