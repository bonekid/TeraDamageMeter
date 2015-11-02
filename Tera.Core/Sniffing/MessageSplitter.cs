﻿// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Tera.PacketLog;

namespace Tera.Sniffing
{
    public class MessageSplitter
    {
        public event Action<Message> MessageReceived;
        private readonly BlockSplitter _clientSplitter = new BlockSplitter();
        private readonly BlockSplitter _serverSplitter = new BlockSplitter();
        private DateTime _time;

        public MessageSplitter()
        {
            _clientSplitter.BlockFinished += ClientBlockFinished;
            _serverSplitter.BlockFinished += ServerBlockFinished;
        }

        void ClientBlockFinished(byte[] block)
        {
            OnMessageReceived(new Message(_time, MessageDirection.ClientToServer, new ArraySegment<byte>(block)));
        }

        void ServerBlockFinished(byte[] block)
        {
            OnMessageReceived(new Message(_time, MessageDirection.ServerToClient, new ArraySegment<byte>(block)));
        }

        protected void OnMessageReceived(Message message)
        {
            Action<Message> handler = MessageReceived;
            if (handler != null)
                handler(message);
        }

        public void ClientToServer(DateTime time, byte[] data)
        {
            _time = time;
            _clientSplitter.Data(data);
            _clientSplitter.PopAllBlocks();
        }

        public void ServerToClient(DateTime time, byte[] data)
        {
            _time = time;
            _serverSplitter.Data(data);
            _serverSplitter.PopAllBlocks();
        }
    }
}
