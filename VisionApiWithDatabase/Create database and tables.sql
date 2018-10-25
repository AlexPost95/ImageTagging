create database ImageTagging;

create table Image (
ID		int		not null	primary key,
Picture	Image	not null
)

create table Tag (
PictureID	int		not null	foreign key references Image(ID),
Tag			varchar(255)	not null
)

create table Description (
PictureID	int		not null	foreign key references Image(ID),
Description			varchar(1000)	not null
)

select I.ID, I.Picture, T.Tag, D.Description
from Description D, Image I inner join Tag T on I.ID = T.PictureID
order by I.ID asc

delete from Tag;
delete from Description;
delete from Image;