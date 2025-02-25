#region

using System.Data.Common;

#endregion

namespace DemoShop.TestUtils.Common.Exceptions;

public class TestDbException(string? message = null) : DbException(message);
