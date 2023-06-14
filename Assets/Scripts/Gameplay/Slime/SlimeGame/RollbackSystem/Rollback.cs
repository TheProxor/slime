using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.RollbackSystem
{
	public class Rollback
	{
		private class Command
		{
			private event Action OnExecute;

			public void AddAction(Action action)
			{
				OnExecute = action + OnExecute;
			}

			public Command(Action onExecute)
			{
				OnExecute += onExecute;
			}

			public void Execute()
			{
				OnExecute?.Invoke();
			}
		}

		public event Action OnPush;
		public event Action OnPop;

		private readonly Stack<Command> commands = new();
		public int StackLength => commands.Count;

		public void PushAction(Action action)
		{
			commands.Push(new Command(action));
			OnPush?.Invoke();
		}

		public void AddActionToLast(Action action)
		{
			commands.TryPeek(out Command command);
			command.AddAction(action);
		}

		public void RemoveLastAction()
		{
			Pop();
		}

		public void Clear()
		{
			commands.Clear();
		}

		public void PerformRollback()
		{
			Pop().Execute();
		}

		private Command Pop()
		{
			Command command = commands.Pop();
			OnPop?.Invoke();

			return command;
		}
	}
}
