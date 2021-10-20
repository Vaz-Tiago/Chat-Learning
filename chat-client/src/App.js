import { useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

import Lobby from './components/Lobby';
import Chat from './components/Chats';

import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css'

const App = () => {
  const [connection, setConnection] = useState();
  const [messages, setMessages] = useState([]);
  const [users, setUsers] = useState([]);

  const joinRoom = async (user, room) => {
    try{

      // Create a connection
      const connection = new HubConnectionBuilder()
        .withUrl('https://localhost:5001/chat')
        .configureLogging(LogLevel.Information)
        .build();

        // Handlers -> Mesmo nome que foi dado no backend - ReceiveMessage
        connection.on('ReceiveMessage', (user, message) => {
          setMessages(messages => [...messages, { user, message }]);
        })

        connection.onclose(e => {
          setConnection();
          setMessages([]);
          setUsers([]);
        })

        connection.on("UsersInRoom", (users) => {
          setUsers(users);
        })

        await connection.start();
        await connection.invoke("JoinRoom", {user, room})
        setConnection(connection);
      
    }catch(e){
      console.log(e);
    }
  }

  const sendMessage = async (message) => {
    try{
      await connection.invoke("SendMessage", message)
    }catch(e){
      console.log(e);
    }
  }

  const closeConnection = async () => {
    try {
      await connection.stop();
    }catch(e){
      console.log(e);
    }
  }

  return (
    <div className='app'>
      <h2>MyChat</h2>
      {!connection
        ? <Lobby joinRoom={joinRoom} />
        : <Chat 
            messages={messages} 
            sendMessage={sendMessage} 
            closeConnection={closeConnection}
            users={users}
          />
      }
    </div>
  )
}

export default App;
