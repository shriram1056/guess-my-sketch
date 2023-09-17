import { Button, Modal, Form } from 'react-bootstrap'
import React from 'react'
import { useState } from 'react'
import './WelcomePage.css'

export function WelcomePage() {
  const [showCreateRoomModal, setShowCreateRoomModal] = useState(false)
  const [formData, setFormData] = useState({}) // State for form data

  const handleNewRoom = () => {
    setShowCreateRoomModal(true) // Show the modal when "Create Room" is clicked
  }
  const handleJoinRoom = () => {
    setShowCreateRoomModal(true) // Show the modal when "Create Room" is clicked
  }

  const handleCreateRoomCloseModal = () => {
    setShowCreateRoomModal(false) // Close the modal
  }

  const handleCreateRoomSubmit = () => {
    console.log(formData)
    handleCreateRoomCloseModal()
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
      <Modal
        show={showCreateRoomModal}
        onHide={handleCreateRoomCloseModal}
        backdrop="static"
      >
        <Modal.Header closeButton>
          <Modal.Title>Create a Room</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="userName">
              <Form.Label>User Name</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter user name"
                onChange={(e) =>
                  setFormData({ ...formData, userName: e.target.value })
                }
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="primary" onClick={handleCreateRoomSubmit}>
            Submit
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  )
}

export default WelcomePage
