create table Channels
(
    Name         varchar(25)      not null,
    Id           bigint default 0 not null
        primary key,
    EmoteInFront varchar(50)      null,
    Prefix       varchar(50)      null
);

create table ExceptionLogs
(
    Id         int auto_increment
        primary key,
    Type       varchar(100)  null,
    Origin     varchar(100)  null,
    Message    varchar(1000) null,
    StackTrace varchar(3000) null
);

create table Reminders
(
    Id      int auto_increment
        primary key,
    Creator varchar(25)      not null,
    Target  varchar(25)      not null,
    Message varchar(500)     not null,
    Channel varchar(25)      not null,
    Time    bigint default 0 not null,
    ToTime  bigint default 0 not null
);

create table Spotify
(
    Id                 bigint auto_increment
        primary key,
    Username           varchar(25)      not null,
    AccessToken        varchar(300)     not null,
    RefreshToken       varchar(300)     not null,
    Time               bigint default 0 not null,
    SongRequestEnabled bit              not null
);

create table Suggestions
(
    Id       int auto_increment
        primary key,
    Username varchar(25)                                      not null,
    Content  varbinary(2000)                                  not null,
    Channel  varchar(50)                                      not null,
    Time     bigint                                           not null,
    Status   enum ('Open', 'Done', 'Rejected') default 'Open' not null
);

create table Users
(
    Id                bigint      default 0  not null
        primary key,
    Username          varchar(25) default '' not null,
    AfkMessage        varchar(500)           null,
    AfkType           int         default 0  not null,
    AfkTime           bigint      default 0  not null,
    IsAfk             bit                    not null,
    Location          varchar(100)           null,
    IsPrivateLocation bit                    not null
);

