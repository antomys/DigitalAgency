using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Action = DigitalAgency.Dal.Entities.Action;

namespace DigitalAgency.Dal.Storages.Interfaces;

public interface IActionStorage
{
    Task CreateActionAsync(Action action);
    Task<List<Action>> GetActionsAsync();
    Task<Action> GetActionAsync(Expression<Func<Action, bool>> expression);
    Task DeleteActionAsync(int id);
    Task UpdateActionAsync(Action action);
}