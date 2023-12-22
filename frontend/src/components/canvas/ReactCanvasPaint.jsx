import React, { useRef, useState, useCallback, useEffect } from 'react'
import PropTypes from 'prop-types'
import classNames from 'classnames'
import styles from './style.module.css'
import EraserIcon from '../../assets/svg/eraser.svg'
import ClearIcon from '../../assets/svg/clear-option-svgrepo-com.svg'

function DrawingToolBox({ colors, active, onChange, onPropChange }) {
  const handleColorClick = (color) => {
    onChange(color)
    onPropChange(color)
  }

  return (
    <div className={styles.toolBoxContainer}>
      <div className={styles.colors}>
        {colors.map((color, key) => (
          <button
            key={key}
            onClick={() => handleColorClick(color)}
            className={classNames(styles.color, {
              [styles.active]: active === color,
            })}
            style={{
              backgroundColor: color,
              pointerEvents: 'auto', // Allow color selection even when eraser is active
            }}
          />
        ))}
      </div>
    </div>
  )
}

function ReactCanvasPaint(props) {
  const canvas = useRef(null)
  const [drawing, setDrawing] = useState(false)
  const [position, setPosition] = useState(null)
  const [activeColor, setActiveColor] = useState(props.colors[0])

  const onDown = useCallback((event) => {
    const coordinates = getCoordinates(event)
    if (coordinates) {
      setPosition(coordinates)
      setDrawing(true)
    }
  }, [])

  const onUp = useCallback(() => {
    setDrawing(false)
    setPosition(null)
  }, [])

  const getCoordinates = (event) => {
    if (!canvas.current) {
      return null
    }

    const canvasRect = canvas.current.getBoundingClientRect()
    const x = event.clientX - canvasRect.left
    const y = event.clientY - canvasRect.top

    return {
      x,
      y,
    }
  }

  const onMove = useCallback(
    (event) => {
      if (drawing) {
        const newPosition = getCoordinates(event)
        if (position && newPosition) {
          drawLine(position, newPosition)
          setPosition(newPosition)
        }
      }
    },
    [drawing, position]
  )

  const drawLine = (originalPosition, newPosition) => {
    if (!canvas.current) {
      return null
    }

    const context = canvas.current.getContext('2d')

    if (context) {
      context.strokeStyle = activeColor
      context.lineJoin = 'round'
      context.lineWidth = props.strokeWidth

      context.beginPath()
      context.moveTo(originalPosition.x, originalPosition.y)
      context.lineTo(newPosition.x, newPosition.y)
      context.closePath()

      context.stroke()

      props.setOriginalPosition(originalPosition)
      props.setNewPosition(newPosition)
    }
  }

  useEffect(() => {
    if (typeof props.data === 'object' && canvas.current) {
      const context = canvas.current.getContext('2d')
      // TODO: scale imageData
      context.putImageData(props.data, 0, 0)
    }
  }, [props.data])

  useEffect(() => {
    if (props.viewOnly) setActiveColor(props.activeColor)
    console.log('colorchange')
  }, [props.activeColor])

  useEffect(() => {
    console.log('reactioon')
    if (props.viewOnly && props.originalPosition && props.newPosition) {
      drawLine(props.originalPosition, props.newPosition)
    }
  }, [props.originalPosition, props.newPosition])

  useEffect(() => {
    if (props.viewOnly && props.clearCanvas && canvas.current) {
      const context = canvas.current.getContext('2d')
      context.clearRect(0, 0, props.width, props.height)
      props.setClearCanvas(false)
    }
  }, [props.clearCanvas])

  return (
    <div className={styles.container}>
      {/* Add a button or icon to toggle eraser */}
      {!props.viewOnly && (
        <>
          <button
            className={styles.eraserButton}
            onClick={() => {
              setActiveColor('#ffffff')
              props.setActiveColor('#ffffff')
            }}
          >
            <img src={EraserIcon} alt="Eraser" className={styles.icon} />
          </button>

          <button
            className={styles.clearButton}
            onClick={() => {
              if (canvas.current) {
                const context = canvas.current.getContext('2d')
                context.clearRect(0, 0, props.width, props.height)
                props.connection.invoke('ClearCanvas', props.room_id)
              }
            }}
          >
            <img src={ClearIcon} alt="Clear" className={styles.icon} />
          </button>
        </>
      )}
      <canvas
        ref={canvas}
        onMouseDown={props.viewOnly ? undefined : onDown}
        onMouseUp={props.viewOnly ? undefined : onUp}
        onMouseLeave={props.viewOnly ? undefined : onUp}
        onMouseMove={props.viewOnly ? undefined : onMove}
        width={props.width}
        height={props.height}
      />
      {!props.viewOnly && (
        <DrawingToolBox
          colors={props.colors}
          active={activeColor}
          onChange={setActiveColor}
          onPropChange={props.setActiveColor}
        />
      )}
    </div>
  )
}

ReactCanvasPaint.propTypes = {
  width: PropTypes.number,
  height: PropTypes.number,
  viewOnly: PropTypes.bool,
  setOriginalPosition: PropTypes.func,
  setNewPosition: PropTypes.func,
  originalPosition: PropTypes.object,
  newPosition: PropTypes.object,
  setActiveColor: PropTypes.func,
  setClearCanvas: PropTypes.func,
  clearCanvas: PropTypes.bool,
  activeColor: PropTypes.string,
  room_id: PropTypes.string,
  connection: PropTypes.object,
  colors: PropTypes.arrayOf(PropTypes.string),
  strokeWidth: PropTypes.number,
}

ReactCanvasPaint.defaultProps = {
  width: 400,
  height: 400,
  viewOnly: false,
  data: undefined,
  onDraw: undefined,
  colors: ['#7030A2', '#000000', '#0170C1', '#FE0002', '#FFFF01', '#00AF52'],
  strokeWidth: 5,
}

export default ReactCanvasPaint
