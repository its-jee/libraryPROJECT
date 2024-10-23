using Library.Data;
using Library.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    [Authorize]

    public class BorrowingController : Controller
    {

        private readonly ApplicationDbContext _context;

        public BorrowingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var borrowings = _context.BorrowingSystems.Include(b => b.BookFk).Include(u => u.UserFk).Where(x=>x.IsReteurned == false).ToList();
            return View(borrowings);
        }

        public IActionResult Create()
        {
            ViewBag.Books = new SelectList(_context.Books.Where(x=>x.CopiesAvailable > 0).ToList(), "Id", "Title");
            ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "UserName");
            BorrowingSystem model = new BorrowingSystem();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(BorrowingSystem borrowing)
        {
            if (ModelState.IsValid)
            {
                var book = _context.Books.Find(borrowing.bookId);
                if (book == null)
                {
                    ModelState.AddModelError(string.Empty, "Book not found.");
                }
                else if (book.CopiesAvailable > 0)
                {
                    // Decrease the available copies
                    book.CopiesAvailable -= 1;
                    _context.Update(book);

                    // Add borrowing to the context
                    _context.BorrowingSystems.Add(borrowing);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index)); // Success case
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No copies available for this book.");
                }
            }

            // If we reach here, something failed, so re-populate ViewBag and return the view
            ViewBag.Books = new SelectList(_context.Books.ToList(), "Id", "Title", borrowing.bookId);
            ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "UserName", borrowing.UserId);
            return View(borrowing); // Return the view when there is a validation issue
        }



        public IActionResult Return(int id)
        {
            var borrowing = _context.BorrowingSystems.Find(id);
            if (borrowing == null) return NotFound();

            ViewBag.Books = new SelectList(_context.Books.ToList(), "Id", "Title", borrowing.bookId);
            ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "UserName", borrowing.UserId);
            return View(borrowing);
        }

        [HttpPost]
        public IActionResult Return(BorrowingSystem borrowinginput)
        {
            var borrowing = _context.BorrowingSystems.Find(borrowinginput.Id);

            if (ModelState.IsValid)
            {
                var book = _context.Books.FirstOrDefault(x => x.Id == borrowing.bookId);

                if (borrowing.ReturnDate.HasValue && borrowing.ReturnDate.Value < DateTime.Now)
                {
                    TimeSpan dateDifference = DateTime.Now.Date - borrowing.ReturnDate.Value.Date;
                    borrowing.Penalty = (float)(book.PenaltyAmount * dateDifference.Days);
                    borrowing.IsReteurned = true;
                }
                else
                {
                    borrowing.Penalty = 0;
                }

                book.CopiesAvailable += 1;
                _context.Update(book);

                _context.Update(borrowing);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Books = new SelectList(_context.Books.ToList(), "Id", "Title", borrowing.bookId);
            ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "UserName", borrowing.UserId);
            return View(borrowing);
        }


        public IActionResult Delete(int id)
        {
            var borrowing = _context.BorrowingSystems
                                    .Include(x => x.BookFk)
                                    .Include(x => x.UserFk)
                                    .FirstOrDefault(x => x.Id == id);
            if (borrowing == null) return NotFound();
            return View(borrowing);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var borrowing = _context.BorrowingSystems.Include(x => x.BookFk).FirstOrDefault(x => x.Id == id);
            if (borrowing != null)
            {
                var book = _context.Books.Find(borrowing.bookId);
                if (book != null)
                {
                    book.CopiesAvailable += 1;
                    _context.Update(book);
                }

                _context.BorrowingSystems.Remove(borrowing);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
