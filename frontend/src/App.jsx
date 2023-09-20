import React from 'react'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import { RoomPage } from './RoomPage'
import { WelcomePage } from './WelcomePage'

export function App() {
  return (
    <Router>
      <Routes>
        <Route path="/room/:id" element={<RoomPage />} />
        <Route path="/" element={<WelcomePage />} />
      </Routes>
    </Router>
  )
}

export default App
