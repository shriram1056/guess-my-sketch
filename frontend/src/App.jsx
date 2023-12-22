import React from 'react'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import { Room } from './pages/Room/Room'
import { Welcome } from './pages/Welcome'

export function App() {
  return (
    <Router>
      <Routes>
        <Route path="/room/:room_id/:username" element={<Room />} />
        <Route path="/" element={<Welcome />} />
      </Routes>
    </Router>
  )
}

export default App
