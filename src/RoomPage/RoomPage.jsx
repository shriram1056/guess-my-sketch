import React, { useState } from 'react'
import './RoomPage.css'

export function RoomPage() {
  const [draw, setDraw] = useState(undefined)
  return (
    <div className="welcome-page d-flex flex-column justify-content-center align-items-center vh-100"></div>
  )
}

export default RoomPage
