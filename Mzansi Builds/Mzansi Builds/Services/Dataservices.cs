using Mzansi_Builds;
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
        private static readonly Lazy<DataService> _lazy = new Lazy<DataService>(() => new DataService(), LazyThreadSafetyMode.ExecutionAndPublication);
        public static DataService Instance => _lazy.Value;

        private readonly List<User> _users = new List<User>();
        private readonly object _usersLock = new object();

        private readonly List<Post> _posts = new List<Post>();
        private readonly object _postsLock = new object();

       

        private string _dataFolder;
        private string _usersFile;
        private string _postsFile;

        private DataService() { }

        public void Initialize(string dataFolder = null)
        {
            _dataFolder = string.IsNullOrWhiteSpace(dataFolder)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MzansiBuilds")
                : dataFolder;

            Directory.CreateDirectory(_dataFolder);

            _usersFile = Path.Combine(_dataFolder, "users.xml");
            _postsFile = Path.Combine(_dataFolder, "posts.xml");

            LoadFromDisk();
            if (!_users.Any())
            {
                _users.AddRange(new List<User>
                {
                    new User { Name = "Alex Johnson", Email = "alex@mail.com", City = "San Francisco, CA", Password = "123", Bio = "Full-stack developer | Open source enthusiast | Coffee lover", JoinedAt = new DateTime(2024,1,15), FollowersCount = 0, FollowingCount = 0, PostsCount = 0 },
                    new User { Name = "Sarah Chen", Email = "sarah@mail.com", City = "New York, NY", Password = "123", Bio = "UI/UX Designer passionate about creating beautiful experiences", JoinedAt = new DateTime(2023,6,2), FollowersCount = 1234, FollowingCount = 567, PostsCount = 10 },
                    new User { Name = "Emily Rodriguez", Email = "emily@mail.com", City = "Seattle, WA", Password = "123", Bio = "Product Manager | Tech innovator | Startup advisor", JoinedAt = new DateTime(2022,3,10), FollowersCount = 200, FollowingCount = 150, PostsCount = 5 },
                    new User { Name = "David Kim", Email = "david@mail.com", City = "Los Angeles, CA", Password = "123", Bio = "DevOps Engineer | Cloud architecture specialist", JoinedAt = new DateTime(2021,11,19), FollowersCount = 95, FollowingCount = 40, PostsCount = 3 },
                    new User { Name = "Marcus Thompson", Email = "marcus@mail.com", City = "Austin, TX", Password = "123", Bio = "Software Engineer | React & TypeScript enthusiast", JoinedAt = new DateTime(2020,9,1), FollowersCount = 892, FollowingCount = 423, PostsCount = 30 }
                });
            }

            if (!_posts.Any())
            {
                _posts.AddRange(new List<Post>
                {
                    new Post
                    {
                        AuthorEmail = "sarah@mail.com",
                        Content = "Just finished redesigning our dashboard 🚀",
                        GithubLink = "https://github.com/example/project",
                        AttachmentPaths = new List<string>(),
                        CreatedAt = DateTime.Now.AddHours(-2),
                        LikesCount = 4
                    },
                    new Post
                    {
                        AuthorEmail = "marcus@mail.com",
                        Content = "Shipping a small utility that saved me hours. Open source link soon!",
                        GithubLink = "",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        LikesCount = 12
                    },
                    new Post
                    {
                        AuthorEmail = "emily@mail.com",
                        Content = "Looking for feedback on this product idea — DM me!",
                        CreatedAt = DateTime.Now.AddMinutes(-45),
                        LikesCount = 2
                    }
                });
            }

            // Remove friend-request seeding if present or leave; unused by feed-only UI
            SaveToDisk();
        }

        // Users ----------------------------------------------------------------

        public IReadOnlyList<User> GetAllUsers()
        {
            lock (_usersLock)
            {
                return _users.ToList().AsReadOnly();
            }
        }

        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            lock (_usersLock)
            {
                return _users.FirstOrDefault(u => string.Equals(u.Email?.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool TryAddUser(User user, out string error)
        {
            error = null;
            if (user == null) { error = "User is null."; return false; }
            if (string.IsNullOrWhiteSpace(user.Email)) { error = "Email is required."; return false; }
            if (string.IsNullOrWhiteSpace(user.Password)) { error = "Password is required."; return false; }

            lock (_usersLock)
            {
                if (_users.Any(u => string.Equals(u.Email?.Trim(), user.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    error = "A user with that email already exists.";
                    return false;
                }

                _users.Add(user);
            }

            SaveToDisk();
            return true;
        }

        public bool ValidateCredentials(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return false;
            lock (_usersLock)
            {
                var user = _users.FirstOrDefault(u => string.Equals(u.Email?.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));
                return user != null && user.Password == password;
            }
        }

        public void RemoveUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;
            lock (_usersLock)
            {
                _users.RemoveAll(u => string.Equals(u.Email?.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            SaveToDisk();
        }

        // Posts ----------------------------------------------------------------

        public IReadOnlyList<Post> GetAllPosts()
        {
            lock (_postsLock)
            {
                return _posts.OrderByDescending(p => p.CreatedAt).ToList().AsReadOnly();
            }
        }

        public void AddPost(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));

            lock (_postsLock)
            {
                post.CreatedAt = DateTime.Now;
                if (post.Id == Guid.Empty) post.Id = Guid.NewGuid();
                _posts.Add(post);
            }

            SaveToDisk();
        }

        public void ToggleLike(Guid postId, string userEmail)
        {
            if (postId == Guid.Empty || string.IsNullOrWhiteSpace(userEmail)) return;

            lock (_postsLock)
            {
                var post = _posts.FirstOrDefault(p => p.Id == postId);
                if (post == null) return;

                if (post.LikedBy == null) post.LikedBy = new List<string>();

                var exists = post.LikedBy.Any(e => string.Equals(e?.Trim(), userEmail?.Trim(), StringComparison.OrdinalIgnoreCase));
                if (exists)
                {
                    post.LikedBy.RemoveAll(e => string.Equals(e?.Trim(), userEmail?.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    post.LikedBy.Add(userEmail);
                }

                post.LikesCount = post.LikedBy.Count;
            }

            SaveToDisk();
        }

        public void ClearAllData()
        {
            lock (_usersLock) _users.Clear();
            lock (_postsLock) _posts.Clear();
            SaveToDisk();
        }

        // Persistence (simple XML) ---------------------------------------------

        private void SaveToDisk()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_dataFolder)) return;

                lock (_usersLock)
                {
                    SerializeToFile(_users, _usersFile);
                }

                lock (_postsLock)
                {
                    SerializeToFile(_posts, _postsFile);
                }
            }
            catch
            {
                // Swallow exceptions for now; in production surface/log appropriately.
            }
        }

        private void LoadFromDisk()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_dataFolder)) return;

                if (File.Exists(_usersFile))
                {
                    var users = DeserializeFromFile<List<User>>(_usersFile);
                    if (users != null)
                    {
                        lock (_usersLock)
                        {
                            _users.Clear();
                            _users.AddRange(users);
                        }
                    }
                }

                if (File.Exists(_postsFile))
                {
                    var posts = DeserializeFromFile<List<Post>>(_postsFile);
                    if (posts != null)
                    {
                        lock (_postsLock)
                        {
                            _posts.Clear();
                            _posts.AddRange(posts);
                        }
                    }
                }
            }
            catch
            {
                // Ignore load errors for now.
            }
        }

        private void SerializeToFile<T>(T data, string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    serializer.Serialize(stream, data);
                }
            }
            catch
            {
                // ignore
            }
        }

        private T DeserializeFromFile<T>(string filePath) where T : class
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return serializer.Deserialize(stream) as T;
                }
            }
            catch
            {
                return null;
            }
        }

    }

}
