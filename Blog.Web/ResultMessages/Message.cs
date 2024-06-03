namespace Blog.Web.ResultMessages
{
    public static class Message
    {
        public static class Article
        {
            public static string Add(string articleTitle)
            {
                return $"{articleTitle} is successfully added.";
            }
            public static string Update(string articleTitle)
            {
                return $"{articleTitle} is successfully updated.";
            }
            public static string Delete(string articleTitle)
            {
                return $"{articleTitle} is successfully deleted.";
            }
            public static string UndoDelete(string articleTitle)
            {
                return $"{articleTitle} is successfully added again.";
            }
        }
        public static class Category
        {
            public static string Add(string categoryTitle)
            {
                return $"{categoryTitle} is successfully added.";
            }
            public static string Update(string categoryTitle)
            {
                return $"{categoryTitle} is successfully updated.";
            }
            public static string Delete(string categoryTitle)
            {
                return $"{categoryTitle} is successfully deleted.";
            }
            public static string UndoDelete(string categoryTitle)
            {
                return $"{categoryTitle} is successfully added again.";
            }
        }


    }
}
