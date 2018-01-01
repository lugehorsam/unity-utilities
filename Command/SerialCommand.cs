﻿namespace Utilities.Command
{
    using System;

    public class SerialCommand : Command
    {
        public override object Current { get; }

        private int _currentIndex;

        protected override bool MoveNextInternal()
        {
            if (_currentIndex >= _commandSteps.Count)
            {
                return false;
            }

            if (!_commandSteps[_currentIndex].MoveNext())
            {
                _currentIndex++;
            }

            return _currentIndex < _commandSteps.Count;
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
