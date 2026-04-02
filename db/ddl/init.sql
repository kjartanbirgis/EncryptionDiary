create table users(
	id uuid not null default gen_random_uuid() primary key,
	username varchar(50) not null unique,
	password_hash bytea not null,
	pw_salt bytea not null,
	pw_iter integer not null,
	created Timestamptz not null default now(),
	updated timestamptz not null default now(),
	deleted timestamptz 
);

create table keys(
	id uuid not null default gen_random_uuid() primary key,
	user_id uuid references users(id),
	enc_key bytea ,
	key_nonce bytea ,
	key_tag bytea ,
	description text,
	sheared bool not null default false,
	
	created Timestamptz not null default now(),
	updated timestamptz not null default now(),
	deleted timestamptz 
);

create table diary(
	id uuid not null default gen_random_uuid() primary key,
	user_id uuid references users(id) not null,
	key_id uuid references keys(id) not null,
	enc_diary_data bytea not null,
	diary_tag bytea not null,
	diary_nonce bytea not null,
	created Timestamptz not null default now(),
	updated timestamptz not null default now(),
	deleted timestamptz 
);

comment on column diary.enc_diary_data is 'json consist of diary,title and dateentry';

create table diary_assets(
	id uuid not null default gen_random_uuid() primary key,
	diary uuid references diary(id),
	key_id uuid references keys(id) not null,
	enc_meta_data bytea not null,
	meta_data_tag bytea not null,
	meta_data_nonce bytea not null,
	enc_content bytea not null,
	content_tag bytea not null,
	content_nonce bytea not null,
	created Timestamptz not null default now(),
	updated timestamptz not null default now(),
	deleted timestamptz 
);
comment on column diary_assets.enc_meta_data is 'json consist of filename,file_extension';