using Mzansi_Builds.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace Mzansi_Builds.Services
{
    public sealed class DataService
    {
        private static readonly Lazy<DataService> _lazy =
            new Lazy<DataService>(() => new DataService(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static DataService Instance => _lazy.Value;

        private readonly List<User> _users = new List<User>();
        private readonly List<Post> _posts = new List<Post>();

        private readonly object _usersLock = new object();
        private readonly object _postsLock = new object();

        private string _dataFolder;
        private string _usersFile;
        private string _postsFile;

        private DataService() { }

        // 🚀 INITIALIZE (CALL THIS ON APP START)
        public void Initialize(string dataFolder = null)
        {
            _dataFolder = string.IsNullOrWhiteSpace(dataFolder)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MzansiBuilds")
                : dataFolder;

            Directory.CreateDirectory(_dataFolder);

            _usersFile = Path.Combine(_dataFolder, "users.xml");
            _postsFile = Path.Combine(_dataFolder, "posts.xml");

            LoadFromDisk();

            // 🔥 ALWAYS ENSURE DUMMY DATA EXISTS
            if (!_posts.Any())
            {
                SeedDummyData();
                SaveToDisk();
            }
        }

        // 👤 USERS ===============================

        public IReadOnlyList<User> GetAllUsers()
        {
            lock (_usersLock)
                return _users.ToList().AsReadOnly();
        }

        public User GetUserByEmail(string email)
        {
            lock (_usersLock)
                return _users.FirstOrDefault(u =>
                    string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryAddUser(User user, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                error = "Email required";
                return false;
            }

            lock (_usersLock)
            {
                if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    error = "User already exists";
                    return false;
                }

                _users.Add(user);
            }

            SaveToDisk();
            return true;
        }

        public bool ValidateCredentials(string email, string password)
        {
            lock (_usersLock)
            {
                return _users.Any(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password);
            }
        }

        // 📝 POSTS ===============================

        public IReadOnlyList<Post> GetAllPosts()
        {
            lock (_postsLock)
                return _posts.OrderByDescending(p => p.CreatedAt).ToList();
        }

        public void AddPost(Post post)
        {
            lock (_postsLock)
            {
                post.CreatedAt = DateTime.Now;

                post.AttachmentPaths = new List<string>();
                post.Likes = new List<string>();
                post.Comments = new List<Comment>();

                _posts.Add(post);
            }

            SaveToDisk();
        }

        public void ToggleLike(Post post, string userEmail)
        {
            lock (_postsLock)
            {
                if (post.Likes.Contains(userEmail))
                    post.Likes.Remove(userEmail);
                else
                    post.Likes.Add(userEmail);
            }

            SaveToDisk();
        }

        public void AddComment(Post post, Comment comment)
        {
            lock (_postsLock)
            {
                post.Comments.Add(comment);
            }

            SaveToDisk();
        }

        // 🔔 NOTIFICATIONS ===============================

        public int GetNotificationCount(string userEmail)
        {
            lock (_postsLock)
            {
                return _posts
                    .Where(p => p.AuthorEmail == userEmail)
                    .Sum(p => p.Likes.Count + p.Comments.Count);
            }
        }

        public List<string> GetNotifications(string userEmail)
        {
            lock (_postsLock)
            {
                return _posts
                    .Where(p => p.AuthorEmail == userEmail)
                    .SelectMany(p =>
                        p.Comments.Select(c => $"{c.AuthorEmail} commented: {c.Text}")
                        .Concat(p.Likes.Select(l => $"{l} liked your post"))
                    )
                    .OrderByDescending(x => x)
                    .Take(20)
                    .ToList();
            }
        }

        // 💾 PERSISTENCE ===============================

        private void SaveToDisk()
        {
            try
            {
                Serialize(_users, _usersFile);
                Serialize(_posts, _postsFile);
            }
            catch { }
        }

        private void LoadFromDisk()
        {
            try
            {
                if (File.Exists(_usersFile))
                {
                    var users = Deserialize<List<User>>(_usersFile);
                    if (users != null) _users.AddRange(users);
                }

                if (File.Exists(_postsFile))
                {
                    var posts = Deserialize<List<Post>>(_postsFile);
                    if (posts != null) _posts.AddRange(posts);
                }
            }
            catch { }
        }

        private void Serialize<T>(T data, string file)
        {
            var serializer = new XmlSerializer(typeof(T));
            var stream = File.Create(file);
            serializer.Serialize(stream, data);
        }

        private T Deserialize<T>(string file)
        {
            var serializer = new XmlSerializer(typeof(T));
            var stream = File.OpenRead(file);
            return (T)serializer.Deserialize(stream);
        }

        // 🔥 DUMMY DATA ===============================

        private void SeedDummyData()
        {
            _posts.Add(new Post
            {
                AuthorEmail = "alex@mail.com",
                Content = "🔥 Just launched my WPF social app!",
                GithubLink = "https://github.com/example/app",
                CreatedAt = DateTime.Now.AddMinutes(-45),
                Likes = new List<string> { "sarah@mail.com", "john@mail.com" },
                Comments = new List<Comment>
                {
                    new Comment
                    {
                        AuthorEmail = "sarah@mail.com",
                        Text = "This is 🔥🔥🔥",
                        CreatedAt = DateTime.Now.AddMinutes(-30)
                    }
                },
                AttachmentPaths = new List<string>()
            });

            _posts.Add(new Post
            {
                AuthorEmail = "sarah@mail.com",
                Content = "Working on UI improvements 🎨",
                CreatedAt = DateTime.Now.AddHours(-2),
                Likes = new List<string>(),
                Comments = new List<Comment>(),
                AttachmentPaths = new List<string>()
            });
        }
    }
}
