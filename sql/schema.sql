-- schema.sql: Portfolio Database
DROP SCHEMA IF EXISTS portfolio CASCADE;
CREATE SCHEMA portfolio;

CREATE TABLE portfolio.course (
    course_id BIGINT GENERATED ALWAYS AS IDENTITY,
    code VARCHAR(20) NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    CONSTRAINT pk_course PRIMARY KEY (course_id),
    CONSTRAINT uq_course_code UNIQUE (code),
    CONSTRAINT chk_course_code_format CHECK (code ~ '^[A-Z]{2,4}[0-9]{3,4}$')
);

CREATE TABLE portfolio.project (
    project_id BIGINT GENERATED ALWAYS AS IDENTITY,
    course_id BIGINT NOT NULL,
    title VARCHAR(150) NOT NULL,
    description TEXT,
    video_url VARCHAR(500),
    CONSTRAINT pk_project PRIMARY KEY (project_id),
    CONSTRAINT fk_project_course FOREIGN KEY (course_id) 
        REFERENCES portfolio.course (course_id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.image (
    image_id BIGINT GENERATED ALWAYS AS IDENTITY,
    project_id BIGINT NOT NULL,
    public_url VARCHAR(500) NOT NULL,
    caption VARCHAR(200),
    CONSTRAINT pk_image PRIMARY KEY (image_id),
    CONSTRAINT fk_image_project FOREIGN KEY (project_id) 
        REFERENCES portfolio.project (project_id) ON DELETE CASCADE,
    CONSTRAINT uq_image_url UNIQUE (public_url),
    CONSTRAINT chk_image_url CHECK (public_url LIKE 'http://%' OR public_url LIKE 'https://%')
);
