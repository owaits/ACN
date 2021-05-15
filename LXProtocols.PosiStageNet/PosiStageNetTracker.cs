using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LXProtocols.PosiStageNet
{
    /// <summary>
    /// A Tracker is a real world coordinate with reference to a specific origin that contains orientation, speed and acceleration. It is used to 
    /// represent a point in real world space for the purpose of controlling stage fixtures. Implementers may use this tracker in different ways to
    /// create a system with undertsanding of movable object on a stage.
    /// </summary>
    public class PosiStageNetTracker
    {
        /// <summary>
        /// Gets or sets a unique identifier for this trackable location.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Gets or sets the friendly name for the tracker.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current position in meters (m) of the tracker.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the speed in meters per second (m/s) of the tracker.
        /// </summary>
        public Vector3 Speed { get; set; }

        /// <summary>
        /// Gets or sets the current orientation in radians of the tracker. The vector expresses the rotation in radians about the xyz axis.
        /// </summary>
        public Vector3 Orientation { get; set; }

        /// <summary>
        /// Gets or sets the acceleration in meters per second squared of the tracker.
        /// </summary>
        public Vector3 Acceleration { get; set; }

        /// <summary>
        /// Gets or sets a position that the target is attempting to reach in meters (m).
        /// </summary>
        /// <remarks>
        /// When a tracker is stationary the target position will be the same as the current tracker position.
        /// </remarks>
        public Vector3 TargetPosition { get; set; }

        /// <summary>
        /// Gets or sets the validity of the tracker.
        /// </summary>
        public float Validity { get; set; }

        /// <summary>
        /// Gets or sets the time stamp taken when the tracker data was calculated. 
        /// </summary>
        /// <remarks>
        /// The timesta,p is expressed in microseconds elapsed since the server was started.
        /// </remarks>
        public TimeSpan TimeStamp { get; set; }

    }
}
