using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;

namespace Library.MVC.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var loans = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member);
            return View(await loans.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (loan == null) return NotFound();

            return View(loan);
        }

        // GET: Loans/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.IsAvailable), "Id", "Title");
            ViewData["MemberId"] = new SelectList(_context.Member, "Id", "Name");
            return View();
        }

        // POST: Loans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,MemberId,LoanDate,DueDate")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                // Check if book is available
                var book = await _context.Books.FindAsync(loan.BookId);
                if (book == null || !book.IsAvailable)
                {
                    ModelState.AddModelError("BookId", "This book is not available.");
                    ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.IsAvailable), "Id", "Title", loan.BookId);
                    ViewData["MemberId"] = new SelectList(_context.Member, "Id", "Name", loan.MemberId);
                    return View(loan);
                }

                book.IsAvailable = false; // mark book as unavailable
                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.IsAvailable), "Id", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Member, "Id", "Name", loan.MemberId);
            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Member, "Id", "Name", loan.MemberId);
            return View(loan);
        }

        // POST: Loans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,MemberId,LoanDate,DueDate,ReturnedDate")] Loan loan)
        {
            if (id != loan.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Member, "Id", "Name", loan.MemberId);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (loan == null) return NotFound();

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan != null)
            {
                // make book available again if it wasn’t returned yet
                if (!loan.ReturnedDate.HasValue && loan.Book != null)
                    loan.Book.IsAvailable = true;

                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Loans/Return/5
        public async Task<IActionResult> Return(int id)
        {
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null) return NotFound();

            loan.ReturnedDate = DateTime.Now;
            if (loan.Book != null)
                loan.Book.IsAvailable = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReturned(int id)
        {
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan != null && loan.ReturnedDate == null)
            {
                loan.ReturnedDate = DateTime.Now;
                loan.Book.IsAvailable = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}