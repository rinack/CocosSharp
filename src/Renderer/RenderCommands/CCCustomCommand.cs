﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CocosSharp
{
    internal class CCCustomCommand : CCRenderCommand
    {
        public Action Action { get; internal set; }

        #region Constructors

        public CCCustomCommand(float globalZOrder, CCAffineTransform worldTransform, Action action) 
            : base(globalZOrder, worldTransform)
        {
            Action = action;
        }

        public CCCustomCommand(float globalZOrder, CCAffineTransform worldTransform) 
            : this(globalZOrder, worldTransform, null)
        {
        }

        #endregion Constructors


        internal override void RequestRenderCommand(CCRenderer renderer)
        {
            if(Action != null)
                renderer.ProcessCustomRenderCommand(this);
        }

        internal void RenderCustomCommand(CCDrawManager drawManager)
        {
            drawManager.PushMatrix();
            drawManager.SetIdentityMatrix();
            var worldTrans = WorldTransform.XnaMatrix;
            drawManager.MultMatrix(ref worldTrans);

            Action();

            drawManager.PopMatrix();
        }
    }
}

