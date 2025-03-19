using System.Collections;

namespace WebApi.Test.InlineData
{
    //class that contains params to the function Error_Empty_Name
    public class CultureInlineDataTest : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "fr" };
            yield return new object[] { "pt-BR" };
            yield return new object[] { "pt-PT" };
            yield return new object[] { "en" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
