/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class SelectorCustomEventWrapper : MonoBehaviour
    {
        [SerializeField, Interface(typeof(ISelector))]
        private MonoBehaviour _selector;

        [SerializeField]
        private Renderer _rendererLeftHand;
        
        [SerializeField]
        private Renderer _rendererRightHand;

        [SerializeField]
        private Material _materialWhenUnselected;
        
        [SerializeField]
        private Material _materialWhenSelected;

        [SerializeField]
        private GroundNormalRotator _groundNormalRotator;
        

        private ISelector Selector;
        private Material _material;
        private bool _selected = false;
        
        protected bool _started = false;

        [SerializeField]
        private ActionOnTimer _actionOnTimer;


        protected virtual void Awake()
        {
            Selector = _selector as ISelector;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Selector);

            Assert.IsNotNull(_rendererRightHand);
            _rendererRightHand.material = _materialWhenUnselected;
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Selector.WhenSelected += HandleSelected;
                Selector.WhenUnselected += HandleUnselected;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                HandleUnselected();
                Selector.WhenSelected -= HandleSelected;
                Selector.WhenUnselected -= HandleUnselected;
            }
        }

        private void OnDestroy()
        {
            Destroy(_material);
        }

        private void HandleSelected()
        {
            if (_selected) return;
            _selected = true;
            

            // Custom functions here
            _actionOnTimer.SetTimer(3.0f, SelectedTimingCallBack);  // when "selected" event begin, count down and execute callback while timer elapsed 
            
        }
        private void HandleUnselected()
        {
            if (!_selected) return;
            _selected = false;
            
            // Custom functions here
            _actionOnTimer.CancelTimer();  // cancel timer if the "selected" event didn't prolong for enough time

        }
        
        private void SelectedTimingCallBack()
        {
            _rendererLeftHand.material = _materialWhenSelected;
            _rendererRightHand.material = _materialWhenSelected;
            _groundNormalRotator.gameObject.SetActive(true); 
        }

        #region Inject

        public void InjectAllSelectorDebugVisual(ISelector selector, Renderer renderer)
        {
            InjectSelector(selector);
            InjectRenderer(renderer);
        }

        public void InjectSelector(ISelector selector)
        {
            _selector = selector as MonoBehaviour;
            Selector = selector;
        }

        public void InjectRenderer(Renderer renderer)
        {
            _rendererRightHand = renderer;
        }

        #endregion
    }
}
