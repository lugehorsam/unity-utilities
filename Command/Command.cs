﻿namespace Utilities.Command
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Holds a queue of enumerators and executes them either in serial or parallel.
    /// </summary>
    public class Command : IEnumerator
    {
        public string Id { get; set; }

        public event CommandCompleteHandler OnComplete = delegate { };

        public delegate void CommandCompleteHandler(Command command);

        private bool _dispatchedComplete;

        /// <summary>
        ///     A list of all serial and parallel commands in the queue. Commands are never removed from this list.
        ///     Instead the <see cref="_queuedCommandIndex" /> increments as it runs commands in sequence.
        /// </summary>
        private readonly List<CommandData> _commands = new List<CommandData>();

        /// <summary>
        ///     All active parallel commands placed from <see cref="_commands" /> that have not yet completed.
        /// </summary>
        private readonly List<CommandData> _activeParallelCommands = new List<CommandData>();

        /// <summary>
        ///     The index of the last run command from <see cref="_commands" />.
        /// </summary>
        private int _queuedCommandIndex;

        /// <summary>
        ///     Whether or not this command queue should reset itself.
        /// </summary>
        private bool _shouldReset;

        /// <summary>
        ///     Unity's Coroutine system handles this property in a special manner. It's best not to deal with this and
        ///     just return null instead.
        ///     See details in https://docs.unity3d.com/ScriptReference/CustomYieldInstruction.html
        /// </summary>
        public object Current => null;

        private bool _IsLastCommand => _queuedCommandIndex == _commands.Count - 1;

        public Command()
        {
            OnComplete += delegate
            {
                _dispatchedComplete = true;
            };
        }

        /// <summary>
        ///     Calls <see cref="MoveNext" /> on the active parallel commands and the active serial command. Only returns
        ///     false if there are no serial and no parallel enumerators left, or if the queue has been <see cref="Reset" />.
        /// </summary>
        public bool MoveNext()
        {
            bool continued = ContinueSubCommands();

            if (continued)
            {
                return true;
            }

            if (!_dispatchedComplete)
            {
                OnComplete(this);
            }

            return false;
        }

        /// <summary>
        ///     Stops and rewinds the queue.
        /// </summary>
        public void Reset()
        {
            _queuedCommandIndex = 0;
            _activeParallelCommands.Clear();
            throw new NotImplementedException("Have not implemented resetting of subcommands.");
        }

        public void Complete()
        {
            CompleteSubCommands();
            ResetForCompletion();
            OnComplete(this);
        }

        public void AddParallel(Action action)
        {
            _commands.Add(new CommandData(action, CommandMode.Parallel));
        }

        /// <summary>
        ///     Enqueues a command that does not block subsequent commands. However, the enumerator must finish in order for
        ///     this command queue to finish.
        /// </summary>
        public void AddParallel(IEnumerator enumerator)
        {
            _commands.Add(new CommandData(enumerator, CommandMode.Parallel));
        }

        public void AddParallel(Func<IEnumerator> enumeratorFunc)
        {
            _commands.Add(new CommandData(enumeratorFunc, CommandMode.Parallel));
        }

        /// <summary>
        ///     Add a blocking command to the queue.
        /// </summary>
        public void AddSerial(IEnumerator enumerator)
        {
            _commands.Add(new CommandData(enumerator, CommandMode.Serial));
        }

        /// <summary>
        ///     Adds a one-shot action to the queue. The action self-evidently not block subsequent commands, but will be
        ///     blocked by previous commands, like any other command.
        /// </summary>
        public void AddSerial(Action action)
        {
            _commands.Add(new CommandData(action, CommandMode.Serial));
        }

        public void AddSerial(Func<IEnumerator> enumeratorFunc)
        {
            _commands.Add(new CommandData(enumeratorFunc, CommandMode.Serial));
        }

        /// <summary>
        ///     Moves all active parallel commands forward one step. Removes completed commands from
        ///     <see cref="_activeParallelCommands" />
        /// </summary>
        private bool ContinueParallelCommands()
        {
            _activeParallelCommands.RemoveAll(command => !command.MoveNext());
            return _activeParallelCommands.Count > 0;
        }

        /// <summary>
        ///     Processes the next command in <see cref="_commands" /> If a parallel command is encountered it is transferred
        ///     to <see cref="_activeParallelCommands" /> and run from there.
        /// </summary>
        private bool ContinueQueuedCommand()
        {
            if (_commands.Count == 0)
            {
                return false;
            }

            CommandData currentQueuedCommand = GetCurrentQueuedCommand();
            
            return ContinueSerialCommand(currentQueuedCommand) || ContinueParallelSubCommands(currentQueuedCommand);
        }

        private bool ContinueSerialCommand(CommandData queuedData)
        {
            Diag.Log("calling continue serial command");
            return (queuedData.CommandMode == CommandMode.Serial) && queuedData.MoveNext();
        }

        private bool ContinueParallelSubCommands(CommandData queuedData)
        {
            bool isParallelCommand = queuedData.CommandMode == CommandMode.Parallel;

            if (isParallelCommand)
            {
                _activeParallelCommands.Add(queuedData);
            }

            return isParallelCommand;
        }

        private void IncrementQueuedCommandIndex()
        {
            _queuedCommandIndex = Math.Min(_queuedCommandIndex + 1, _commands.Count - 1);
        }

        private void ResetForCompletion()
        {
            _queuedCommandIndex = _commands.Count;
            _activeParallelCommands.Clear();
        }

        private void CompleteSubCommands()
        {
            CompleteCurrentCommand();
            CompleteParallelCommands();
        }

        private void CompleteCurrentCommand()
        {
            IEnumerator currentEnumerator = _commands[_queuedCommandIndex];
            var currentCommand = currentEnumerator as Command;
            currentCommand?.Complete();
        }

        private void CompleteParallelCommands()
        {
            IEnumerable<Command> activeParallelCommands = _activeParallelCommands.OfType<Command>();

            foreach (Command activeParallelCommand in activeParallelCommands)
            {
                activeParallelCommand.Complete();
            }
        }

        private bool ContinueSubCommands()
        {
            bool continuedParallelCommands = ContinueParallelCommands();
            bool continuedQueuedCommand = ContinueQueuedCommand();
            bool shouldContinue = continuedParallelCommands || continuedQueuedCommand || !_IsLastCommand;
            
            if (!continuedQueuedCommand)
            {
                IncrementQueuedCommandIndex();
            }

            return shouldContinue;
        }

        private CommandData GetCurrentQueuedCommand()
        {
            try
            {
                CommandData queuedCommandData = _commands[_queuedCommandIndex];
                return queuedCommandData;
            }
            catch (ArgumentOutOfRangeException)
            {
                string msg = $"Could not index command at {_queuedCommandIndex} from list of length {_commands.Count}";
                throw new IndexOutOfRangeException(msg);
            }
        }
    }
}
