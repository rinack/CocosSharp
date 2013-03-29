﻿
namespace cocos2d
{
    public class CCCallFunc : CCActionInstant
    {
        private SEL_CallFunc m_pCallFunc;
        protected string m_scriptFuncName;

        public CCCallFunc()
        {
            m_scriptFuncName = "";
            m_pCallFunc = null;
        }

        public static CCCallFunc Create(SEL_CallFunc selector)
        {
            var pRet = new CCCallFunc();
            pRet.m_pCallFunc = selector;
            return pRet;
        }

        public virtual void Execute()
        {
            if (null != m_pCallFunc)
            {
                m_pCallFunc();
            }
            //if (m_nScriptHandler) {
            //    CCScriptEngineManager::sharedManager()->getScriptEngine()->executeCallFuncActionEvent(this);
            //}
        }

        public override void Update(float time)
        {
            Execute();
        }

        public override object Copy(ICopyable pZone)
        {
            CCCallFunc pRet;

            if (pZone != null)
            {
                //in case of being called at sub class
                pRet = (CCCallFunc) (pZone);
            }
            else
            {
                pRet = new CCCallFunc();
                pZone =  (pRet);
            }

            base.Copy(pZone);
            pRet.m_pCallFunc = m_pCallFunc;
            pRet.m_scriptFuncName = m_scriptFuncName;
            return pRet;
        }
    }
}