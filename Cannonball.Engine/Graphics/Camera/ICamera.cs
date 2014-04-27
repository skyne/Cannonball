using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Graphics.Camera
{
    public interface ICamera
    {
        Vector3 Position { get; set; }
        Vector3 Target { get; set; }
        Vector3 Up { get; set; }

        Matrix ViewMatrix { get; }
        Matrix ProjectionMatrix { get; }
    }

    public abstract class CameraBase : ICamera
    {
        protected Matrix view;
        protected bool viewDirty;
        protected Matrix projection;
        protected bool projectionDirty;

        public Matrix ViewMatrix
        {
            get 
            {
                if (viewDirty)
                {
                    view = CalculateViewMatrix();
                    viewDirty = false;
                }
                return view;
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                if (projectionDirty)
                {
                    projection = CalculateProjection();
                    projectionDirty = false;
                }
                return projection;
            }
        }

        protected Vector3 _pos;
        public virtual Vector3 Position
        {
            get { return _pos; }
            set { _pos = value; viewDirty = true; }
        }

        protected Vector3 _target;
        public virtual Vector3 Target
        {
            get { return _target; }
            set { _target = value; viewDirty = true; }
        }

        protected Vector3 _up;
        public virtual Vector3 Up
        {
            get { return _up; }
            set { _up = value; viewDirty = true; }
        }

        protected virtual Matrix CalculateViewMatrix()
        {
            return Matrix.CreateLookAt(Position, Target, Up);
        }
        protected abstract Matrix CalculateProjection();
    }
}