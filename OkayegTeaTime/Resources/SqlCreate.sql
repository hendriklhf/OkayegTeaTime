create table Channels
(
    Id                 int auto_increment
        primary key,
    ChannelName        varchar(50)      not null,
    EmoteInFront       varbinary(100)   null,
    Prefix             varbinary(50)    null,
    EmoteManagementSub bit default b'0' not null
);

create table Reminders
(
    Id       int auto_increment
        primary key,
    FromUser varchar(50)      not null,
    ToUser   varchar(50)      not null,
    Message  varbinary(2000)  not null,
    Channel  varchar(50)      not null,
    Time     bigint default 0 not null,
    ToTime   bigint default 0 not null
);

create table Spotify
(
    Id                 int auto_increment
        primary key,
    Username           varchar(50)         not null,
    AccessToken        varchar(300)        not null,
    RefreshToken       varchar(300)        not null,
    Time               bigint default 0    not null,
    SongRequestEnabled bit    default b'0' null
);

create table Suggestions
(
    Id         int auto_increment
        primary key,
    Username   varchar(50)                                      not null,
    Suggestion varbinary(2000)                                  not null,
    Channel    varchar(50)                                      not null,
    Time       bigint                                           not null,
    Status     enum ('Open', 'Done', 'Rejected') default 'Open' not null
);

create table Users
(
    Id         int         default 0    not null
        primary key,
    Username   varchar(50) default ''   not null,
    AfkMessage varbinary(2000)          null,
    AfkType    int         default 0    not null,
    AfkTime    bigint      default 0    not null,
    IsAfk      bit         default b'0' not null
);

