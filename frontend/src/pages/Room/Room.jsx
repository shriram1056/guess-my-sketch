import React, { useState, useEffect } from 'react'
import ReactCanvasPaint from '../../components/canvas/ReactCanvasPaint'
import {
  HubConnectionBuilder,
  LogLevel,
  HttpTransportType,
} from '@microsoft/signalr'
import './Room.css'
import { useParams } from 'react-router-dom'

export function Room() {
  const [originalPosition, setOriginalPosition] = useState(undefined)
  const [newPosition, setNewPosition] = useState(undefined)
  const [activeColor, setActiveColor] = useState(undefined)
  const [gameStarted, setGameStarted] = useState(false)
  const [pause, setPause] = useState(false)
  const [host, setHost] = useState(false)
  const [currentDrawer, setCurrentDrawer] = useState(false)
  const [clearCanvas, setClearCanvas] = useState(false)
  const [messages, setMessages] = useState([])
  const [newMessage, setNewMessage] = useState('')
  const [scoreboardData, setScoreboardData] = useState([])
  const [connection, setConnection] = useState(null)
  const [countdown, setCountdown] = useState(60)
  const { room_id, username } = useParams()

  useEffect(() => {
    const socketConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Debug)
      .withUrl('http://localhost:5027/room', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
        withCredentials: true,
      })
      .build()
    setConnection(socketConnection)
  }, [])

  useEffect(() => {
    if (connection) {
      connection.on('host', () => {
        setHost(true)
      })
      connection.on('GameStarted', () => {
        setPause(false)
        setGameStarted(true)
      })
      connection.on('ResetTimer', () => {
        setPause(false)
        setCountdown(60)
      })
      connection.on('ReceiveCanvasData', (data) => {
        setOriginalPosition(data.originalPosition)
        setNewPosition(data.newPosition)
      })
      connection.on('ReceiveActiveColor', (activeColor) => {
        setActiveColor(activeColor)
      })
      connection.on('CurrentDrawer', (name) => {
        setCurrentDrawer(true)
        let word = prompt('what will you be drawing?')
        connection.invoke('NewWord', word)
      })
      connection.on('EndGame', (name) => {
        alert(`the winner of the game is ${name}`)
      })
      connection.on('GetInfo', (data) => {
        setPause(true)
        setCurrentDrawer(false)
        setScoreboardData(data)
      })
      connection.on('ClearCanvas', () => {
        setClearCanvas(true)
      })
      connection.on('error', (message) => {
        alert(message)
      })
      connection.start().then(function () {
        console.log('connected')
      })
    }
  }, [connection])

  useEffect(() => {
    if (connection) {
      connection.on('ReceiveMessage', (data) => {
        setMessages(messages.concat(data))
      })
    }
  }, [connection, messages])
  const handleSendMessage = () => {
    connection.invoke('SendMessage', newMessage, countdown, false)
    setNewMessage('')
  }

  useEffect(() => {
    if (connection && currentDrawer) {
      connection.invoke(
        'SendCanvasData',
        originalPosition,
        newPosition,
        room_id
      )
    }
  }, [originalPosition, newPosition, currentDrawer])

  useEffect(() => {
    if (connection) {
      connection.invoke('SendActiveColor', activeColor, room_id)
    }
  }, [activeColor])

  React.useEffect(() => {
    if (gameStarted && !pause) {
      countdown > 0 && setTimeout(() => setCountdown(countdown - 1), 1000)
      if (countdown == 0 && currentDrawer) {
        connection.invoke('NextRound', username)
      }
    }
  }, [countdown, gameStarted])

  const getUsernameColor = (str) => {
    let hash = 0
    str.split('').forEach((char) => {
      hash = char.charCodeAt(0) + ((hash << 5) - hash)
    })
    let colour = '#'
    for (let i = 0; i < 3; i++) {
      const value = (hash >> (i * 8)) & 0xff
      colour += value.toString(16).padStart(2, '0')
    }
    return colour
  }

  return (
    <div className="container-fluid d-flex flex-column vh-100 p-2 roomPageContainer">
      <div className="row d-flex justify-content-between align-items-center">
        {host && !gameStarted && (
          <button
            className="btn btn-primary"
            onClick={() => {
              let word = prompt('what will you be drawing?')
              setCurrentDrawer(true)
              connection.invoke('NewWord', word)
              connection.invoke('GameStarted')
            }}
          >
            Start Game
          </button>
        )}
        <div>
          <p>
            {countdown < 10 ? '0' : ''}
            {countdown} seconds left
          </p>
        </div>
      </div>
      <div className="row">
        <div className="canvas col-auto ms-3 p-0">
          <ReactCanvasPaint
            strokeWidth={15}
            viewOnly={!currentDrawer}
            width={700}
            height={300}
            setOriginalPosition={setOriginalPosition}
            originalPosition={originalPosition}
            setActiveColor={setActiveColor}
            activeColor={activeColor}
            setNewPosition={setNewPosition}
            newPosition={newPosition}
            clearCanvas={clearCanvas}
            setClearCanvas={setClearCanvas}
            connection={connection}
            room_id={room_id}
          />
        </div>
        <div className="col d-flex flex-column chats bg-primary me-3 pb-3">
          <h3 className="chat-title text-center m-3 text-white ">Chats</h3>
          <div className="chat-container mx-4 bg-white p-3 flex-basis-1">
            {messages.map((message, index) => {
              const userColorStyle = { color: getUsernameColor(message.name) }
              if (message.special) {
                return (
                  <p key={index} className="mb-2" style={userColorStyle}>
                    {message.name + ' ' + message.message}
                  </p>
                )
              } else {
                return (
                  <p key={index} className="mb-2">
                    <span style={userColorStyle}>{message.name}</span>
                    {' ' + message.message}
                  </p>
                )
              }
            })}
          </div>
          {gameStarted && (
            <div class="mx-4 d-flex">
              <input
                type="text"
                class="flex-grow-1"
                value={newMessage}
                onChange={(e) => setNewMessage(e.target.value)}
                placeholder="Type your message..."
              />
              <button onClick={handleSendMessage}>Send</button>
            </div>
          )}
        </div>
      </div>
      <div className="row flex-grow-1 scoreboard">
        <div className="col my-4">
          <h2 className="mb-4 text-center">Scoreboard</h2>
          <div className="scoreboard-table-container">
            <table className="table table-bordered table-striped">
              <thead>
                <tr>
                  <th>Rank</th>
                  <th>Player Name</th>
                  <th>Score</th>
                </tr>
              </thead>
              <tbody>
                {scoreboardData.map((player, index) => (
                  <tr key={index}>
                    <td>{index + 1}</td>
                    <td>{player.name}</td>
                    <td>{player.score}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Room
