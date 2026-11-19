INSERT INTO portfolio.course (code, name, semester, academic_year, description) VALUES
('PROGIT1', 'Programming 1', '1st semester', '2025-2026', 'Introduction to programming using C#'),
('DBMWEB1', 'Database Management and Web Systems', '1st semester', '2026-2027', 'Relational databases and ASP.NET Core MVC');

INSERT INTO portfolio.project (course_id, title, description, completed_date, video_url) VALUES
(1, 'Grade Calculator', 'A C# console program that reads quiz, project and exam scores and computes the final grade.', '2025-10-10', NULL),
(2, 'Library System', 'An ASP.NET Core MVC application on PostgreSQL: books, members and loans.', '2026-11-06', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ'),
(2, 'This Portfolio', 'The site you are viewing — courses, projects and images.', CURRENT_DATE, NULL);

INSERT INTO portfolio.image (project_id, public_url, caption) VALUES
(1, 'https://i.imgur.com/6WmXQfL.png', 'Calculator Icon');
