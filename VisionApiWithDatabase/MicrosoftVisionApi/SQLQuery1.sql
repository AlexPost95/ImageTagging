create database ImageTagging;

use ImageTagging

create table ImagesWithTags (
picture	image			not null,
tags	varchar(max)	null
);

insert into ImagesWithTags values('C:\Users\alex.post\Downloads\scenery.jpg', 'water');
insert into ImagesWithTags values('C:\Users\alex.post\Downloads\scenery.jpg', 'outdoor');
insert into ImagesWithTags values('C:\Users\alex.post\Downloads\scenery.jpg', 'nature');
insert into ImagesWithTags values('C:\Users\alex.post\Downloads\scenery.jpg', 'lake');
insert into ImagesWithTags values('C:\Users\alex.post\Downloads\scenery.jpg', 'boat');
insert into ImagesWithTags values('C:\Users\alex.post\Downloads\scenery.jpg', 'mountain');
insert into ImagesWithTags values('C:\Users\alex.post\Downloads\scenery.jpg', 'river');
insert into ImagesWithTags values('C:\Users\alex.post\Downloads\scenery.jpg', 'waterfall');

select * from ImagesWithTags where tags like '%boat%'
select * from ImagesWithTags

/* delete from ImagesWithTags */

create table Images (
ID		int		not null	Primary key,
Picture	Image	not null	
)

create table Tags(
PictureId	int				not null Foreign key references Images(id) on update cascade on delete cascade,
Tag			varchar(255)	not null
)

insert into Images values (1, 'C:/Users/alex.post/Documents/Alex Post/Programmeeropdrachten/Image tagging/MicrosoftVisionApi/MicrosoftVisionApi/scenery.jpg');
insert into Tags values(1, 'Person');
insert into Tags values(1, 'Water');
insert into Tags values(1, 'Boat');
insert into Tags values(1, 'Sailing');
insert into Tags values(1, 'Waves');

select I.id, I.Picture, T.tag from Images I inner join Tags T on I.id = T.pictureId;
delete from Images where id = 1;
drop table Tags;
drop table Images;
