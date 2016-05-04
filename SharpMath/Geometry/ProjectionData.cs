// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Drawing;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Provides configuration data for performing projections in a view frustum.
    /// </summary>
    [Serializable]
    public struct ProjectionData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectionData" /> struct.
        /// </summary>
        /// <param name="canvasSize">The size of the canvas.</param>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <param name="fieldOfView">The field of view.</param>
        /// <param name="nearPlane">The near plane.</param>
        /// <param name="farPlane">The far plane.</param>
        /// <exception cref="ArgumentException">
        ///     nearPlane must be a value greater than zero.
        ///     or
        ///     farPlane must be a value greater than zero.
        ///     or
        ///     nearPlane must be a value smaller than farPlane.
        ///     or
        ///     FOV must be greater than zero and must not be bigger than 360 degrees (PI).
        /// </exception>
        public ProjectionData(Size canvasSize, float aspectRatio, float fieldOfView, float nearPlane, float farPlane)
        {
            if (nearPlane <= 0)
                throw new ArgumentException("nearPlane must be a value greater than zero.");
            if (farPlane <= 0)
                throw new ArgumentException("farPlane must be a value greater than zero.");
            if (nearPlane >= farPlane)
                throw new ArgumentException("nearPlane must be a value smaller than farPlane.");
            if (fieldOfView <= 0 || fieldOfView > Math.PI)
                throw new ArgumentException(
                    "FOV must be greater than zero and must not be bigger than 360 degrees (PI).");

            CanvasSize = canvasSize;
            AspectRatio = aspectRatio;
            FieldOfView = fieldOfView;
            NearPlane = nearPlane;
            FarPlane = farPlane;
        }

        /// <summary>
        ///     Gets or sets the size of the canvas.
        /// </summary>
        public Size CanvasSize { get; set; }

        /// <summary>
        ///     Gets or sets the aspect ratio.
        /// </summary>
        public float AspectRatio { get; set; }

        /// <summary>
        ///     Gets or sets the field of view (FOV).
        /// </summary>
        public float FieldOfView { get; set; }

        /// <summary>
        ///     Gets or sets the near plane.
        /// </summary>
        public float NearPlane { get; set; }

        /// <summary>
        ///     Gets or sets the far plane.
        /// </summary>
        public float FarPlane { get; set; }

        /// <summary>
        ///     Gets the depth.
        /// </summary>
        public float Depth => FarPlane - NearPlane;

        /// <summary>
        ///     Gets the default <see cref="ProjectionData" />.
        /// </summary>
        public static ProjectionData Default
            => new ProjectionData(new Size(100, 100), 16f/9f, (float) Math.PI/3f, 1f, 100f);
    }
}