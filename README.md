# 📌 Mzansi_Builds
## 🧾 Project Description

Mzansi_Builds is a C# WPF-based social platform application that allows users to create posts, interact through comments, receive notifications, and manage user profiles. The system is designed using an MVC (Model–View–Controller) architecture to ensure clean separation of concerns, scalability, and maintainability.

## 🚀 Features
### 👤 User Management
1. User registration and profile management
2. View and update user information
### 📝 Posts
1. Create new posts
2. View posts from users
3. Attach images to posts
### 💬 Comments
1. Add comments to posts
2. View all comments under a post
### 🔔 Notifications
1. Receive notifications for interactions (e.g. comments)
2. View notification list
### 🖼️ Image Attachments
1. Upload and display images on posts
2. File validation for safe uploads
   
## 🏗️ Architecture

The project follows the MVC (Model–View–Controller) pattern:

### 📊 Model
Represents core data structures:
1. User
2. Post
3. Comment
4. Notification
### 🖥️ View
Built using WPF (Windows Presentation Foundation) Includes:
1. ProfileWindow
2. FriendsWindow
3. NotificationWindow
4. Post UI components
### ⚙️ Controller (Service Layer)
Handles business logic:
1. UserService
2. PostService
3. CommentService
4. NotificationService
Manages validation and system rules
### 🔄 System Flow
1. User interacts with the WPF interface (View)
2. Request is sent to the Controller (Service Layer)
3. Business rules are applied (validation, authorization)
4. Data is processed in Models
5. Updated results are returned to the View
🧰 Technologies Used
1. C#
2. .NET Framework / .NET (WPF)
3. XAML (UI Design)
4. MVC Architecture Pattern
5. Git & GitHub (Version Control)
🔐 Security Considerations
Input validation for posts and comments
File type restrictions for image uploads
Ownership checks for editing/deleting content
Notification duplication prevention logic
