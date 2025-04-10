using CashFlow.Domain.Security.Cryptography;
using Moq;

namespace CommonTestUtilities.Cryptography
{
    public class PasswordEncrypterBuilder
    {
        private readonly Mock<IPasswordEncripter> _mock;

        public PasswordEncrypterBuilder()
        {
            _mock = new Mock<IPasswordEncripter>();

            _mock.Setup(passwordEncripter => passwordEncripter.Encrypt(It.IsAny<string>())).Returns("!%idndkhid545");

        }

        public PasswordEncrypterBuilder Verify(string? password)
        {

            if(string.IsNullOrEmpty(password) == false)
            {
                _mock.Setup(passwordEncripter => passwordEncripter.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            }

            return this;
        }

        public IPasswordEncripter Build() => _mock.Object;
        
    }
}
