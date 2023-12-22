import { Button, Modal, Form } from 'react-bootstrap'
import React from 'react'
import { useNavigate } from 'react-router-dom'
import { useState } from 'react'
import axios from 'axios'
import './Welcome.css'

export function Welcome() {
  const navigate = useNavigate()
  const [showNewRoomModal, setShowNewRoomModal] = useState(false)
  const [newRoomFormData, setNewRoomFormData] = useState({}) // State for form data

  const [showJoinRoomModal, setShowJoinRoomModal] = useState(false)
  const [joinRoomFormData, setJoinRoomFormData] = useState({}) // State for form data

  const handleNewRoom = () => {
    setShowNewRoomModal(true) // Show the modal when "New Room" is clicked
  }
  const handleJoinRoom = () => {
    setShowJoinRoomModal(true) // Show the modal when "New Room" is clicked
  }

  const handleNewRoomCloseModal = () => {
    setShowNewRoomModal(false) // Close the modal
  }

  const handleJoinRoomCloseModal = () => {
    setShowJoinRoomModal(false) // Close the modal
  }

  const handleNewRoomSubmit = () => {
    const config = {
      withCredentials: true,
    }

    axios
      .post('http://localhost:5027/createRoom', newRoomFormData, config)
      .then(function (response) {
        navigate(`/room/${response.data.code}/${response.data.username}`)
      })
      .catch(function (error) {
        console.error('Error:', error)
      })

    handleNewRoomCloseModal()
  }

  const handleJoinRoomSubmit = () => {
    const config = {
      withCredentials: true,
    }

    axios
      .post('http://localhost:5027/joinRoom', joinRoomFormData, config)
      .then(function (response) {
        navigate(`/room/${response.data.code}/${response.data.username}`)
      })
      .catch(function (error) {
        console.error('Error:', error)
      })
    handleJoinRoomCloseModal()
  }

  return (
    <div className="welcome-page d-flex flex-column justify-content-center align-items-center vh-100">
      <h1 className="fw-bold text-white mb-5 display-4">Welcome to the Game</h1>
      <div className="row">
        <div className="col-auto">
          <button className="btn btn-primary fw-bold" onClick={handleNewRoom}>
            New Room
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
      <Modal
        show={showNewRoomModal}
        onHide={handleNewRoomCloseModal}
        backdrop="static"
      >
        <Modal.Header closeButton>
          <Modal.Title>New a Room</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="userName">
              <Form.Label>User Name</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter user name"
                required
                onChange={(e) =>
                  setNewRoomFormData({
                    ...newRoomFormData,
                    name: e.target.value,
                  })
                }
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="primary" onClick={handleNewRoomSubmit}>
            Submit
          </Button>
        </Modal.Footer>
      </Modal>

      <Modal
        show={showJoinRoomModal}
        onHide={handleJoinRoomCloseModal}
        backdrop="static"
        className="d-flex flex-column justify-content-center modal"
      >
        <Modal.Header closeButton>
          <Modal.Title>New a Room</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="userName" className="mb-4">
              <Form.Label>User Name</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter user name"
                required
                onChange={(e) =>
                  setJoinRoomFormData({
                    ...joinRoomFormData,
                    name: e.target.value,
                  })
                }
              />
            </Form.Group>
            <Form.Group controlId="roomCode">
              <Form.Label>Room Code</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter room code"
                required
                onChange={(e) =>
                  setJoinRoomFormData({
                    ...joinRoomFormData,
                    code: e.target.value,
                  })
                }
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="primary" onClick={handleJoinRoomSubmit}>
            Submit
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  )
}

export default Welcome
