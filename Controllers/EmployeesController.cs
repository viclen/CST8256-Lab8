using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab5.Models;
using Lab5.Models.DataAccess;

namespace Lab5.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly StudentRecordContext _context;

        public EmployeesController(StudentRecordContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var list = _context.Employee.Include(e => e.EmployeeRole).ThenInclude(e => e.Role);
            return View(await list.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            EmployeeRoleSelections employeeRoleSelections = new EmployeeRoleSelections();
            return View(employeeRoleSelections);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeRoleSelections employeeRoleSelections)
        {
            if (!employeeRoleSelections.roleSelections.Any(m => m.Selected))
            {
                ModelState.AddModelError("roleSelections", "You must select at least one role!");
            }
            if (_context.Employee.Any(e => e.UserName == employeeRoleSelections.employee.UserName))
            {
                ModelState.AddModelError("employee.UserName", "This user name already exists!");
            }
            if (ModelState.IsValid)
            {
                _context.Add(employeeRoleSelections.employee);
                _context.SaveChanges();
                foreach(RoleSelection roleSelection in employeeRoleSelections.roleSelections)
                {
                    if (roleSelection.Selected)
                    {
                        EmployeeRole employeeRole = new EmployeeRole { RoleId = roleSelection.role.Id, EmployeeId = employeeRoleSelections.employee.Id };
                        _context.EmployeeRole.Add(employeeRole);
                    }
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employeeRoleSelections);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var roles = _context.Role.ToList();
            var roleSelections = new List<RoleSelection>();
            foreach(Role role in roles)
            {
                bool selected = false;
                if(_context.EmployeeRole.Any(e => e.EmployeeId==id && e.RoleId == role.Id))
                {
                    selected = true;
                }
                roleSelections.Add(new RoleSelection(role,selected));
            }

            EmployeeRoleSelections employeeRoleSelections = new EmployeeRoleSelections { employee = employee, roleSelections = roleSelections };
            return View(employeeRoleSelections);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeRoleSelections employeeRoleSelections)
        {
            if (!employeeRoleSelections.roleSelections.Any(m => m.Selected))
            {
                ModelState.AddModelError("roleSelections", "You must select at least one role!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var allEmployeeRoles = _context.EmployeeRole.Where(e => e.EmployeeId == id).ToList();
                    if (allEmployeeRoles.Count > 0)
                    {
                        _context.RemoveRange(allEmployeeRoles);
                    }

                    foreach (RoleSelection roleSelection in employeeRoleSelections.roleSelections)
                    {
                        if (roleSelection.Selected)
                        {
                            _context.EmployeeRole.Add(new EmployeeRole { EmployeeId = id, RoleId = roleSelection.role.Id });
                        }
                    }

                    _context.Update(employeeRoleSelections.employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employeeRoleSelections);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var roles = _context.Role.ToList();
            var roleSelections = new List<RoleSelection>();
            foreach (Role role in roles)
            {
                bool selected = false;
                if (_context.EmployeeRole.Any(e => e.EmployeeId == id && e.RoleId == role.Id))
                {
                    selected = true;
                }
                roleSelections.Add(new RoleSelection(role, selected));
            }

            EmployeeRoleSelections employeeRoleSelections = new EmployeeRoleSelections { employee = employee, roleSelections = roleSelections };
            return View(employeeRoleSelections);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeRoleSelections = _context.EmployeeRole.Where(e => e.EmployeeId == id).ToList();
            _context.EmployeeRole.RemoveRange(employeeRoleSelections);
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}
