-- seed.sql: Sample data
INSERT INTO portfolio.course (code, name, description) VALUES
 ('CS101', 'Introduction to Computer Science', 'Fundamentals of programming and computational thinking'),
 ('DB202', 'Database Management Systems', 'Relational databases, SQL, and design principles'),
 ('WD301', 'Web Application Development', 'Building dynamic web apps with ASP.NET Core');

INSERT INTO portfolio.project (course_id, title, description, video_url) VALUES
 (1, 'First Console Program', 'My very first C# console application.', ''),
 (2, 'Library Database Design', 'Designed the schema for a library management system.', ''),
 (3, 'Student Record System', 'ASP.NET MVC app for managing student data.', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ');

INSERT INTO portfolio.image (project_id, public_url, caption) VALUES
 (2, 'https://images.unsplash.com/photo-1532372320572-cda25653a68d', 'ERD Diagram'),
 (2, 'https://images.unsplash.com/photo-1550745165-9bc0b252726f', 'Schema Implementation');

-- Verify
SELECT 'Courses' AS table_name, COUNT(*) AS rows FROM portfolio.course
UNION ALL
SELECT 'Projects', COUNT(*) FROM portfolio.project
UNION ALL
SELECT 'Images', COUNT(*) FROM portfolio.image;
