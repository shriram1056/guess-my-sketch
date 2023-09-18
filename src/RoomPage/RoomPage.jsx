import React, { useState } from 'react'
import ReactCanvasPaint from './ReactCanvasPaint'
import './RoomPage.css'

export function RoomPage() {
  const [draw, setDraw] = useState(undefined)
  console.log(draw)
  return (
    <div className="d-flex flex-column justify-content-center align-items-center vh-100">
      <div className="canvas">
        <ReactCanvasPaint
          strokeWidth={15}
          width={400}
          height={400}
          onDraw={setDraw}
        />
      </div>
      <div className="canvas">
        <ReactCanvasPaint
          viewOnly
          strokeWidth={15}
          width={400}
          height={400}
          data={draw}
        />
      </div>
    </div>
  )
}

export default RoomPage
