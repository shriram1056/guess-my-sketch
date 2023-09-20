import React, { useState, useEffect } from 'react'
import ReactCanvasPaint from './ReactCanvasPaint'
import './RoomPage.css'

export function RoomPage() {
  const [draw, setDraw] = useState(undefined)
  const [scoreboardData, setScoreboardData] = useState([
    { name: 'Player 1', score: 100 },
    { name: 'Player 2', score: 85 },
    { name: 'Player 3', score: 70 },
    // Add more objects as needed
  ])

  useEffect(() => {
    // Sort the scoreboardData array based on score in descending order
    const sortedData = [...scoreboardData].sort((a, b) => b.score - a.score)
    setScoreboardData(sortedData)
  }, [])

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

  console.log(getUsernameColor('User2'))

  return (
    <div className="container-fluid d-flex flex-column vh-100 p-2 roomPageContainer">
      <div className="row">
        <div className="canvas col-auto ms-3 p-0">
          <ReactCanvasPaint
            strokeWidth={15}
            width={800}
            height={400}
            onDraw={setDraw}
          />
        </div>
        <div className="col d-flex flex-column chats bg-primary me-3 pb-3">
          {' '}
          <h3 className="chat-title text-center m-3 text-white ">Chats</h3>
          <div className="chat-container mx-4 bg-white p-3 flex-basis-1">
            {/* Sample chat messages */}
            <p className="mb-2" style={{ color: getUsernameColor('User 1') }}>
              User 1: Hello!
            </p>
            <p className="mb-2">User 2: Hi there!</p>
            {/* Add more chat messages as needed */}
          </div>
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

export default RoomPage
