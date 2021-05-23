using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.EntityFrameworkCore;
using Action = DigitalAgency.Dal.Entities.Action;

namespace DigitalAgency.Dal.Storages
{
    public class ActionStorage : IActionStorage
    {
        private readonly ServicingContext _context;
        
        public ActionStorage(ServicingContext context)
        {
            _context = context;

        }
        
        public async Task CreateActionAsync(Action action)
        {
            _context.Actions.Add(action);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Action>> GetActionsAsync()
        {
            return await _context.Actions.OrderByDescending(x => x.ActionDate).ToListAsync();
        }

        public async Task<Action> GetActionAsync(Expression<Func<Action, bool>> expression)
        {
            return await _context.Actions.OrderByDescending(x => x.ActionDate).FirstOrDefaultAsync(expression);
        }

        public async Task DeleteActionAsync(int id)
        {
            _context.Actions.Remove(await _context.Actions.FindAsync(id));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActionAsync(Action action)
        {
            _context.Actions.Update(action);
            await _context.SaveChangesAsync();
        }
    }
}