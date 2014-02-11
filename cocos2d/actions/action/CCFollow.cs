using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace CocosSharp
{
    public class CCFollow : CCAction
    {
        internal struct CCFollowBoundary
        {
            internal float BottomBoundary { get; private set; } 
            internal float LeftBoundary { get; private set; }
            internal float RightBoundary { get; private set; }  
            internal float TopBoundary { get; private set; } 

            internal CCFollowBoundary(float bottomB, float leftB, float rightB, float topB) : this()
            {
                BottomBoundary = bottomB;
                LeftBoundary = leftB;
                RightBoundary = rightB;
                TopBoundary = topB;
            }
        }

        /// <summary>
        /// whether camera should be limited to certain area
        /// </summary>
        public bool BoundarySet { get; private set; }
        public bool BoundaryFullyCovered { get; private set; }
        internal CCFollowBoundary Boundary { get; private set; }

        internal CCPoint FullScreenSize { get; private set; }
        internal CCPoint HalfScreenSize { get; private set; }

        protected internal CCNode FollowedNode { get; private set; }

        // Take me out later - See comments in CCAction
        public override bool HasState 
        { 
            get { return true; }
        }


        #region Constructors

        public CCFollow(CCNode followedNode, CCRect rect)
        {
            Debug.Assert(followedNode != null);

            FollowedNode = followedNode;
            if (rect.Equals(CCRect.Zero))
            {
                BoundarySet = false;
            }
            else
            {
                BoundarySet = true;
            }

            BoundaryFullyCovered = false;

            CCSize winSize = CCDirector.SharedDirector.WinSize;
            FullScreenSize = (CCPoint)winSize;
            HalfScreenSize = FullScreenSize * 0.5f;

            if (BoundarySet)
            {
                float leftBoundary = -((rect.Origin.X + rect.Size.Width) - FullScreenSize.X);
                float rightBoundary = -rect.Origin.X;
                float topBoundary = -rect.Origin.Y;
                float bottomBoundary = -((rect.Origin.Y + rect.Size.Height) - FullScreenSize.Y);

                if (rightBoundary < leftBoundary)
                {
                    // screen width is larger than world's boundary width
                    //set both in the middle of the world
                    rightBoundary = leftBoundary = (leftBoundary + rightBoundary) / 2;
                }
                if (topBoundary < bottomBoundary)
                {
                    // screen width is larger than world's boundary width
                    //set both in the middle of the world
                    topBoundary = bottomBoundary = (topBoundary + bottomBoundary) / 2;
                }

                if ((topBoundary == bottomBoundary) && (leftBoundary == rightBoundary))
                {
                    BoundaryFullyCovered = true;
                }

                Boundary = new CCFollowBoundary(bottomBoundary, leftBoundary, rightBoundary, topBoundary);
            }
        }

        #endregion Constructors


        protected internal override CCActionState StartAction(CCNode target)
        {
            return new CCFollowState(this, target);
        }
    }


    #region Action state

    public class CCFollowState : CCActionState
    {
        CCFollow.CCFollowBoundary _cachedBoundary;
        CCPoint _cachedHalfScreenSize;

        protected CCFollow FollowAction
        {
            get { return Action as CCFollow; }
        }

        public override bool IsDone
        {
            get { return !FollowAction.FollowedNode.IsRunning; }
        }

        public CCFollowState(CCFollow action, CCNode target) : base(action, target)
        {
            // Cache these structs so we don't have to get them at each running step
            CCFollow followAction = FollowAction;
            _cachedBoundary = followAction.Boundary;
            _cachedHalfScreenSize = followAction.HalfScreenSize;
        }

        public override void Stop()
        {
            Target = null;
            base.Stop();
        }

        public override void Step(float dt)
        {
            CCFollow followAction = FollowAction;
            CCPoint followedNodePos = followAction.FollowedNode.Position;

            if (followAction.BoundarySet)
            {
                // whole map fits inside a single screen, no need to modify the position - unless map boundaries are increased
                if (followAction.BoundaryFullyCovered)
                {
                    return;
                }

                CCPoint tempPos = _cachedHalfScreenSize - followedNodePos;

                Target.Position = new CCPoint(
                    MathHelper.Clamp(tempPos.X, _cachedBoundary.LeftBoundary, _cachedBoundary.RightBoundary),
                    MathHelper.Clamp(tempPos.Y, _cachedBoundary.BottomBoundary, _cachedBoundary.TopBoundary)
                );
            }
            else
            {
                Target.Position = _cachedHalfScreenSize - followedNodePos;
            }
        }
    }

    #endregion Action state
}