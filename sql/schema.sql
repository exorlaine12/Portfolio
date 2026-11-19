DROP SCHEMA IF EXISTS portfolio CASCADE;
CREATE SCHEMA portfolio;

CREATE TABLE portfolio.course (
    course_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code VARCHAR(20) NOT NULL,
    name VARCHAR(100) NOT NULL,
    semester VARCHAR(50),
    academic_year VARCHAR(20),
    description TEXT,
    CONSTRAINT uq_course_code UNIQUE (code)
);

CREATE TABLE portfolio.project (
    project_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    course_id BIGINT NOT NULL REFERENCES portfolio.course(course_id) ON DELETE RESTRICT,
    title VARCHAR(150) NOT NULL,
    description TEXT,
    completed_date DATE,
    video_url VARCHAR(500),
    CONSTRAINT chk_video_url CHECK (video_url IS NULL OR video_url LIKE 'https://%.%')
);

CREATE TABLE portfolio.image (
    image_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    project_id BIGINT NOT NULL REFERENCES portfolio.project(project_id) ON DELETE CASCADE,
    public_url VARCHAR(500) NOT NULL,
    caption VARCHAR(200),
    CONSTRAINT uq_image_url UNIQUE (public_url),
    CONSTRAINT chk_image_url CHECK (public_url LIKE 'http://%' OR public_url LIKE 'https://%')
);
