import { Button, Modal, Form } from 'react-bootstrap'
import React from 'react'
import { useState } from 'react'
import './WelcomePage.css'

export function WelcomePage() {
  const [showNewRoomModal, setShowNewRoomModal] = useState(false)
  const [newRoomFormData, setNewRoomFormData] = useState({}) // State for form data

  const handleNewRoom = () => {
    setShowNewRoomModal(true) // Show the modal when "New Room" is clicked
  }
  const handleJoinRoom = () => {
    setShowNewRoomModal(true) // Show the modal when "New Room" is clicked
  }

  const handleNewRoomCloseModal = () => {
    setShowNewRoomModal(false) // Close the modal
  }

  const handleNewRoomSubmit = () => {
    console.log(newRoomFormData)
    handleNewRoomCloseModal()
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
                onChange={(e) =>
                  setNewRoomFormData({
                    ...newRoomFormData,
                    userName: e.target.value,
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
    </div>
  )
}

export default WelcomePage
