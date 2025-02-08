#region

using System.Data.Common;
using System.Data.Entity.Infrastructure;

#endregion

namespace DemoShop.TestUtils.Common.Exceptions;

public class TestDbException(string? message = null) : DbException(message);
