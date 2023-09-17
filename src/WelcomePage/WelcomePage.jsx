import React from 'react'
import { useState } from 'react'
import './WelcomePage.css'

export function WelcomePage() {
  const [showCreateRoomModal, setShowCreateRoomModal] = useState(false)

  const handleNewRoom = () => {
    setShowCreateRoomModal(true) // Show the modal when "Create Room" is clicked
  }
  const handleJoinRoom = () => {
    setShowCreateRoomModal(true) // Show the modal when "Create Room" is clicked
  }

  return (
    <div className="welcome-page d-flex flex-column justify-content-center align-items-center vh-100">
      <h1 className="fw-bold text-white mb-5 display-4">Welcome to the Game</h1>
      <div className="row">
        <div className="col-auto">
          <button className="btn btn-primary fw-bold" onClick={handleNewRoom}>
            Create Room
          </button>
        </div>
        <div className="col-auto">
          <button
            className="btn btn-secondary fw-bold"
            onClick={handleJoinRoom}
          >
            Join Room
          </button>
        </div>
      </div>
    </div>
  )
}

export default WelcomePage
