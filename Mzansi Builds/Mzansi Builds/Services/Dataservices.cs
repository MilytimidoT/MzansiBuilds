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

        private readonly List<FriendRequest> _requests = new List<FriendRequest>();
        private readonly object _requestsLock = new object();

        private string _dataFolder;
        private string _usersFile;
        private string _postsFile;

        private DataService() { }

        /// <summary>
        /// Call once at application startup to configure persistence and load saved data.
        /// If you don't want persistence, you can skip calling Initialize().
        /// </summary>
        public void Initialize(string dataFolder = null)
        {
            _dataFolder = string.IsNullOrWhiteSpace(dataFolder)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MzansiBuilds")
                : dataFolder;

            Directory.CreateDirectory(_dataFolder);

            _usersFile = Path.Combine(_dataFolder, "users.xml");
            _postsFile = Path.Combine(_dataFolder, "posts.xml");

            LoadFromDisk();
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

        /// <summary>
        /// Adds a user if the email does not already exist. Returns false and sets error on failure.
        /// This implementation expects the caller to set the user's Password (current plaintext model).
        /// Consider replacing plaintext storage with a secure hash (see project notes).
        /// </summary>
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
                _posts.Add(post);
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
        public void SendFriendRequest(string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || from == to)
                return;

            lock (_requestsLock)
            {
                if (_requests.Any(r =>
                    r.FromEmail == from &&
                    r.ToEmail == to))
                    return;

                _requests.Add(new FriendRequest
                {
                    FromEmail = from,
                    ToEmail = to,
                    IsAccepted = false
                });
            }
        }
        public List<FriendRequest> GetPendingRequests(string email)
        {
            lock (_requestsLock)
            {
                return _requests
                    .Where(r => r.ToEmail == email && !r.IsAccepted)
                    .ToList();
            }
        }
        public void AcceptRequest(FriendRequest request)
        {
            lock (_requestsLock)
            {
                var req = _requests.FirstOrDefault(r =>
                    r.FromEmail == request.FromEmail &&
                    r.ToEmail == request.ToEmail);

                if (req != null)
                    req.IsAccepted = true;
            }
        }
        public List<string> GetFriends(string email)
        {
            lock (_requestsLock)
            {
                return _requests
                    .Where(r => r.IsAccepted &&
                           (r.FromEmail == email || r.ToEmail == email))
                    .Select(r => r.FromEmail == email ? r.ToEmail : r.FromEmail)
                    .Distinct()
                    .ToList();
            }
        }
        public List<User> SearchUsers(string currentEmail)
        {
            lock (_usersLock)
            {
                return _users
                    .Where(u => u.Email != currentEmail)
                    .ToList();
            }
        }
    }
}