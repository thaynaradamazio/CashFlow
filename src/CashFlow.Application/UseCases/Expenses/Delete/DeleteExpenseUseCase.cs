using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Repositories;
using CashFlow.Exception.ExceptionsBase;
using CashFlow.Exception;
using CashFlow.Domain.Services.LoggedUser;

namespace CashFlow.Application.UseCases.Expenses.Delete
{
    public class DeleteExpenseUseCase : IDeleteExpenseUseCase
    {
        private readonly IExpensesReadOnlyRepository _expensesReadOnly;
        private readonly IExpensesWriteOnlyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggedUser _loggedUser;

        public DeleteExpenseUseCase(
        IExpensesWriteOnlyRepository repository,
        IExpensesReadOnlyRepository expensesReadOnly,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _loggedUser = loggedUser;
            _expensesReadOnly = expensesReadOnly;
        }

        public async Task Execute(long id)
        {
            var loggedUser = await _loggedUser.Get();

            var expense = await _expensesReadOnly.GetById(loggedUser, id);

            if (expense is null)
            {
                throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
            }

            await _repository.Delete(id);

            await _unitOfWork.Commit();
        }
    }
}
